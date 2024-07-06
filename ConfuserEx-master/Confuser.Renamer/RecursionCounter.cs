using System;
using System.Globalization;

// This file is originally from dnlib. Find the original source here:
// https://github.com/0xd4d/dnlib/blob/a75105a4600b5641e42e6ac36847661ae9383701/src/DotNet/RecursionCounter.cs
// Find the original license of this file here:
// https://github.com/0xd4d/dnlib/blob/a75105a4600b5641e42e6ac36847661ae9383701/LICENSE.txt
namespace Confuser.Renamer {
	/// <summary>
	/// Recursion counter
	/// </summary>
	internal ref struct RecursionCounter {
		/// <summary>
		/// Max recursion count. If this is reached, we won't continue, and will use a default value.
		/// </summary>
		private const int MAX_RECURSION_COUNT = 100;

		/// <summary>
		/// Gets the recursion counter
		/// </summary>
		private int Counter { get; set; }

		/// <summary>
		/// Increments <see cref="Counter"/> if it's not too high. <c>ALL</c> instance methods
		/// that can be called recursively must call this method and <see cref="Decrement"/>
		/// (if this method returns <see langword="true" />)
		/// </summary>
		/// <returns><see langword="true" /> if it was incremented and caller can continue, <see langword="false" /> if
		/// it was <c>not</c> incremented and the caller must return to its caller.</returns>
		public bool Increment() {
			if (Counter >= MAX_RECURSION_COUNT)
				return false;
			Counter++;
			return true;
		}

		/// <summary>
		/// Must be called before returning to caller if <see cref="Increment"/>
		/// returned <see langword="true" />.
		/// </summary>
		public void Decrement() {
#if DEBUG
			if (Counter <= 0)
				throw new InvalidOperationException("recursionCounter <= 0");
#endif
			Counter--;
		}

		/// <inheritdoc/>
		public override string ToString() => Counter.ToString(CultureInfo.InvariantCulture);
	}
}
