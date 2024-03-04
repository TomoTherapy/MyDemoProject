using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CommonUtilities
{
	/// <summary>
	/// A pool that contains bitmaps instances. Get instance with GetInstance() and return used bitmap instace with ReturnInstance().
	/// This is for extreme optimization. Use it well.
	/// </summary>
	public class BitmapPool
	{
		private Queue<Bitmap> mBitmaps;
		private uint mCapacity;
		private uint mCount;
		private uint mLimit;
		private int mWidth;
		private int mHeight;
		private bool mIsAlive;
		private PixelFormat mFormat;
		private ColorPalette mColorPalette;

		public uint Count { get => mCount; }
		public uint Capacity { get => mCapacity; }
		public uint Limit { get => mLimit; set => mLimit = value; }
		public int Width { get => mWidth; }
		public int Height { get => mHeight; }
		public bool IsAlive { get => mIsAlive; }

		public BitmapPool(int width, int height, PixelFormat format, uint capacity, uint limit)
		{
			Debug.Assert(width > 0 || height > 0);
			Debug.Assert(capacity > 0 || limit > 0);
			Debug.Assert(format != PixelFormat.DontCare);
			Debug.Assert(capacity < limit);

			mBitmaps = new Queue<Bitmap>();

			for (int i = 0; i < capacity; ++i)
			{
				Bitmap bmp = new Bitmap(width, height, format);

				// Indexed bitmap needs to specify its palette. We all know that, right?
				if (format == PixelFormat.Format8bppIndexed)
				{
					if (mColorPalette == null)
					{
						mColorPalette = bmp.Palette;
						for (int j = 0; j < 256; ++j)
						{
							mColorPalette.Entries[j] = Color.FromArgb(255, j, j, j);
						}
						bmp.Palette = mColorPalette;
					}
					else
					{
						bmp.Palette = mColorPalette;
					}
				}

				mBitmaps.Enqueue(bmp);
			}

			mCount = capacity;
			mCapacity = capacity;
			mLimit = limit;
			mWidth = width;
			mHeight = height;
			mIsAlive = true;
			mFormat = format;
		}

		/// <summary>
		/// Get instance from the pool.
		/// </summary>
		/// <returns>Bitmap object</returns>
		/// <exception cref="">
		/// Throws exception when available count is zero and max capacity reaches limit.
		/// </exception>
		public Bitmap GetInstance()
		{
			if (mCount == 0)
			{
				if (mCapacity == mLimit)
				{
					throw new InvalidOperationException("Capacity limit reaches");
				}

				mCapacity++;
				return new Bitmap(mWidth, mHeight, mFormat);
			}
			else
			{
				mCount--;
				return mBitmaps.Dequeue();
			}
		}

		/// <summary>
		/// Get instance from the pool.
		/// And copy image data from the IntPtr.
		/// No additional memory allocation.
		/// </summary>
		/// <returns>Bitmap object</returns>
		/// <exception cref="InvalidOperationException">
		/// Throws exception when available count is zero and max capacity reaches limit.
		/// </exception>
		public Bitmap GetInstance(IntPtr pointer)
		{
			Debug.Assert(pointer != IntPtr.Zero);
			Bitmap bmp;
			if (mCount == 0)
			{
				if (mCapacity == mLimit)
				{
					throw new InvalidOperationException("Capacity limit reaches");
				}

				mCapacity++;
				bmp = new Bitmap(mWidth, mHeight, mFormat);
			}
			else
			{
				mCount--;
				bmp = mBitmaps.Dequeue();
			}

			BitmapData data = bmp.LockBits(new Rectangle(0, 0, mWidth, mHeight), ImageLockMode.ReadWrite, mFormat);
			int size = data.Stride * mHeight;

			Utilities.MemCopyMultiThread(data.Scan0, pointer, size);

			bmp.UnlockBits(data);

			return bmp;
		}

		/// <summary>
		/// Get instance from the pool.
		/// And load the image from the path.
		/// Temporal memory allocation.
		/// </summary>
		/// <returns>Bitmap object</returns>
		/// <exception cref="InvalidOperationException">
		/// Throws exception when available count is zero and max capacity reaches limit.
		/// Throws exception when path is not available
		/// </exception>
		public Bitmap GetInstance(string path)
		{
			Debug.Assert(path != null);
			if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
			{
				throw new InvalidOperationException("Path is not valid");
			}

			Bitmap bmp;
			if (mCount == 0)
			{
				if (mCapacity == mLimit)
				{
					throw new InvalidOperationException("Capacity limit reaches");
				}

				mCapacity++;
				bmp = new Bitmap(mWidth, mHeight, mFormat);
			}
			else
			{
				mCount--;
				bmp = mBitmaps.Dequeue();
			}

			using (Bitmap temp = new Bitmap(path))
			{
				Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

				// Lock bits for both bitmaps
				BitmapData tempData = temp.LockBits(rect, ImageLockMode.ReadOnly, temp.PixelFormat);
				BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

				// Copy pixel data from temp to existing Bitmap
				int size = tempData.Stride * temp.Height;
				Utilities.MemCopyMultiThread(bmpData.Scan0, tempData.Scan0, size);

				// Unlock bits
				temp.UnlockBits(tempData);
				bmp.UnlockBits(bmpData);
			}

			return bmp;
		}

		/// <summary>
		/// Return Bitmap instance to the pool for future use
		/// </summary>
		/// <param name="bmp"></param>
		public void ReturnInstance(Bitmap bmp)
		{
			Debug.Assert(bmp != null);
			mBitmaps.Enqueue(bmp);
			mCount++;
		}

		/// <summary>
		/// Release all instances from memory.
		/// </summary>
		public void Release()
		{
			while (mBitmaps.Count > 0)
			{
				mBitmaps.Dequeue().Dispose();
			}

			mBitmaps = null; // then one day GC will collect the remaining
			mCount = 0;
			mCapacity = 0;
			mLimit = 0;
			mWidth = 0;
			mHeight = 0;
			mIsAlive = false;
			mFormat = PixelFormat.DontCare;
		}
	}
}
