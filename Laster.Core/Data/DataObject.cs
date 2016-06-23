﻿using Laster.Core.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace Laster.Core.Data
{
    /// <summary>
    /// Objeto
    /// </summary>
    public class DataObject : IData, IEnumerable<object>
    {
        /// <summary>
        /// Datos
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Origen de datos</param>
        /// <param name="data">Datos</param>
        public DataObject(IDataSource source, object data) : base(source) { Data = data; }

        public override object GetInternalObject() { return Data; }

        IEnumerator<object> GetEmpty()
        {
            if (Data != null) yield return Data;
        }
        public IEnumerator<object> GetEnumerator()
        {
            if (Data != null)
            {
                if (Data is IEnumerator<object>)
                    return (IEnumerator<object>)Data;
            }
            return GetEmpty();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (Data != null)
            {
                if (Data is IEnumerator)
                    return (IEnumerator)Data;
            }
            return GetEmpty();
        }
    }
}