using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace CommonUtilities
{
	/// <summary>
	/// Some common functions for use
	/// </summary>
	public class Utilities
	{
		[DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
		public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

		/// <summary>
		/// Memory copy by using Marshal class
		/// </summary>
		/// <param name="dest"></param>
		/// <param name="src"></param>
		/// <param name="size"></param>
		public static void MemCopyMarshal(IntPtr dest, IntPtr src, int size)
		{
			byte[] vals = new byte[size];
			Marshal.Copy(src, vals, 0, size);
			Marshal.Copy(vals, 0, dest, size);
		}

		/// <summary>
		/// Memory copy by using kernel32.dll external C function
		/// </summary>
		/// <param name="dest"></param>
		/// <param name="src"></param>
		/// <param name="size"></param>
		public static void MemCopyKernal32(IntPtr dest, IntPtr src, int size)
		{
			CopyMemory(dest, src, (uint)size);
		}

		/// <summary>
		/// Momory copy by unsafe direct memory manipulation with pointers
		/// </summary>
		/// <param name="dest"></param>
		/// <param name="src"></param>
		/// <param name="size"></param>
		public static void MemCopyUnsafePointer(IntPtr dest, IntPtr src, int size)
		{
			unsafe
			{
				byte* srcPtr = (byte*)src.ToPointer();
				byte* destPtr = (byte*)dest.ToPointer();

				for (int i = 0; i < size; i++)
				{
					*destPtr++ = *srcPtr++;
				}
			}
		}

		/// <summary>
		/// Memory copy by unsafe direct memory manipulation with multiple threads.
		/// </summary>
		/// <param name="dest"></param>
		/// <param name="src"></param>
		/// <param name="size"></param>
		public static void MemCopyMultiThread(IntPtr dest, IntPtr src, int size)
		{
			int threadCount = 1000;

			int partSize = size / threadCount;
			CountdownEvent countdownEvent = new CountdownEvent(threadCount);

			int j = 0;
			for (int i = 0; i < threadCount; ++i)
			{
				ThreadPool.QueueUserWorkItem(o =>
				{
					int idx = j++;
					int startIdx = idx * partSize;
					int endIdx = idx * partSize + partSize;

					if (startIdx >= size)
					{
						startIdx = size - 1;
					}

					if (endIdx >= size)
					{
						endIdx = size - 1;
					}

					if (idx == threadCount - 1)
					{
						if (endIdx != size - 1)
						{
							endIdx = size - 1;
						}
					}

					if (startIdx != endIdx)
					{
						MemCopyUnsafePointer(dest + startIdx, src + startIdx, endIdx - startIdx);
					}
					countdownEvent.Signal();
				});
			}

			countdownEvent.Wait(200);
		}
	}
}
