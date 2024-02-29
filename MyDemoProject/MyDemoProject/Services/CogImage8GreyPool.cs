using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using Cognex.VisionPro;
using System.Diagnostics;
using System.IO;

namespace CommonUtilities
{
	public class CogImage8GreyPool
	{
		private Queue<CogImage8Grey> mCogImages;
		private uint mCapacity;
		private uint mCount;
		private uint mLimit;
		private int mWidth;
		private int mHeight;
		private bool mIsAlive;

		public uint Count { get => mCount; }
		public uint Capacity { get => mCapacity; }
		public uint Limit { get => mLimit; set => mLimit = value; }
		public int Width { get => mWidth; }
		public int Height { get => mHeight; }
		public bool IsAlive { get => mIsAlive; }

		public CogImage8GreyPool(int width, int height, uint capacity, uint limit)
		{
			Debug.Assert(width > 0 || height > 0);
			Debug.Assert(capacity > 0 || limit > 0);
			Debug.Assert(capacity < limit);

			mCogImages = new Queue<CogImage8Grey>();

			for (int i = 0; i < capacity; ++i)
			{
				CogImage8Grey image = new CogImage8Grey(width, height);
				mCogImages.Enqueue(image);
			}

			mCount = capacity;
			mCapacity = capacity;
			mLimit = limit;
			mWidth = width;
			mHeight = height;
			mIsAlive = true;
		}

		/// <summary>
		/// Get empty instance
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public CogImage8Grey GetInstance()
		{
			if (mCount == 0)
			{
				if (mCapacity == mLimit)
				{
					throw new InvalidOperationException("Capacity limit reaches");
				}

				mCapacity++;
				return new CogImage8Grey(mWidth, mHeight);
			}
			else
			{
				mCount--;
				return mCogImages.Dequeue();
			}
		}

		/// <summary>
		/// Get instance of CogImage8Grey from the pool
		/// Direct image data copy from bitmap to CogImage
		/// No additional memory allocation
		/// </summary>
		/// <param name="bmp"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public CogImage8Grey GetInstance(Bitmap bmp)
		{
			Debug.Assert(bmp != null);

			CogImage8Grey cogImage;
			if (mCount == 0)
			{
				if (mCapacity == mLimit)
				{
					throw new InvalidOperationException("Capacity limit reaches");
				}

				mCapacity++;
				cogImage = new CogImage8Grey(mWidth, mHeight);
			}
			else
			{
				mCount--;
				cogImage = mCogImages.Dequeue();
			}

			BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
			ICogImage8PixelMemory pixelMemory = cogImage.Get8GreyPixelMemory(CogImageDataModeConstants.ReadWrite, 0, 0, cogImage.Width, cogImage.Height);

			int size = data.Stride * data.Height;

			Utilities.MemCopyMultiThread(pixelMemory.Scan0, data.Scan0, size);

			pixelMemory.Dispose();
			bmp.UnlockBits(data);

			return cogImage;
		}

		/// <summary>
		/// Get instance of CogImage8Grey from the pool.
		/// Direct memory copy from the pointer.
		/// No additional memory allocation.
		/// </summary>
		/// <param name="ptr"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public CogImage8Grey GetInstance(IntPtr ptr)
		{
			Debug.Assert(ptr != IntPtr.Zero);

			CogImage8Grey cogImage;
			if (mCount == 0)
			{
				if (mCapacity == mLimit)
				{
					throw new InvalidOperationException("Capacity limit reaches");
				}

				mCapacity++;
				cogImage = new CogImage8Grey(mWidth, mHeight);
			}
			else
			{
				mCount--;
				cogImage = mCogImages.Dequeue();
			}

			ICogImage8PixelMemory pixelMemory = cogImage.Get8GreyPixelMemory(CogImageDataModeConstants.ReadWrite, 0, 0, cogImage.Width, cogImage.Height);
			int size = cogImage.Width * cogImage.Height;
			Utilities.MemCopyMultiThread(pixelMemory.Scan0, ptr, size);
			pixelMemory.Dispose();

			return cogImage;
		}

		/// <summary>
		/// Get instance of CogImage8Grey from the pool
		/// If it exceed limit, throws exception
		/// Temporal memory allocation
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">
		/// When capacity limit reaches.
		/// </exception>
		public CogImage8Grey GetInstance(string path)
		{
			Debug.Assert(path != null);
			if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
			{
				throw new InvalidOperationException("Path is not valid");
			}

			CogImage8Grey cogImage;
			if (mCount == 0)
			{
				if (mCapacity == mLimit)
				{
					throw new InvalidOperationException("Capacity limit reaches");
				}

				mCapacity++;
				cogImage = new CogImage8Grey(mWidth, mHeight);
			}
			else
			{
				mCount--;
				cogImage = mCogImages.Dequeue();
			}

			using (Bitmap bmp = new Bitmap(path))
			{
				BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
				ICogImage8PixelMemory pixelMemory = cogImage.Get8GreyPixelMemory(CogImageDataModeConstants.ReadWrite, 0, 0, cogImage.Width, cogImage.Height);

				int size = data.Stride * data.Height;

				Utilities.MemCopyMultiThread(pixelMemory.Scan0, data.Scan0, size);

				pixelMemory.Dispose();
				bmp.UnlockBits(data);
			}

			return cogImage;
		}

		/// <summary>
		/// Returns an instance to the pool for future use
		/// </summary>
		/// <param name="cogImage"></param>
		public void ReturnInstance(CogImage8Grey cogImage)
		{
			Debug.Assert(cogImage != null);

			mCogImages.Enqueue(cogImage);
			mCount++;
		}

		/// <summary>
		/// Release all memory
		/// </summary>
		public void Release()
		{
			while (mCogImages.Count > 0)
			{
				mCogImages.Dequeue().Dispose();
			}

			mCount = 0;
			mCapacity = 0;
			mLimit = 0;
			mWidth = 0;
			mHeight = 0;
			mIsAlive = false;
		}
	}
}
