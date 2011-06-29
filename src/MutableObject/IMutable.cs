using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MutableObject
{
    /// <summary>
    /// Interface used on all mutable types
    /// </summary>
    /// <typeparam name="T">The mutable type.</typeparam>
    public interface IMutable<T>
    {
        /// <summary>
        /// Get the mutator for the mutable type.
        /// </summary>
        Mutator<T> Mutator { get; }
    }
}
