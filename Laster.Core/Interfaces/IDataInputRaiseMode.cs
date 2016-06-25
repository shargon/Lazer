﻿using System.ComponentModel;
using System.Drawing;

namespace Laster.Core.Interfaces
{
    public class IDataInputRaiseMode
    {
        /// <summary>
        /// Devuelve si está iniciado o no
        /// </summary>
        [Browsable(false)]
        public bool IsStarted { get; private set; }
        /// <summary>
        /// Constructor privado
        /// </summary>
        internal protected IDataInputRaiseMode() { }

        public virtual void Start(IDataInput input) { IsStarted = true; }
        public virtual void Stop(IDataInput input) { IsStarted = false; }
        public virtual Image GetIcon() { return null; }
    }
}