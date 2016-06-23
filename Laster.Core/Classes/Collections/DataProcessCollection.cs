﻿using Laster.Core.Data;
using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laster.Core.Classes.Collections
{
    public class DataProcessCollection : IDataCollection<IDataProcess>
    {
        bool _UseParallel;
        IDataSource _Parent;

        /// <summary>
        /// Origen de datos
        /// </summary>
        public IDataSource Parent { get { return _Parent; } }
        /// <summary>
        /// Usar procesamiento en paralelo si o no
        /// </summary>
        public bool UseParallel { get { return _UseParallel; } set { _UseParallel = value; } }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Padre</param>
        public DataProcessCollection(IDataSource parent)
        {
            _Parent = parent;
        }
        // Cuando se añade o elimina un Origen a esta colección se le añade como origen esperado
        protected override void OnItemAdd(IDataProcess item) { item.Data.Add(_Parent); }
        protected override void OnItemRemove(IDataProcess item) { item.Data.Remove(_Parent); }
        /// <summary>
        /// Lanza el evento de creación
        /// </summary>
        public void RaiseOnCreate()
        {
            foreach (IDataProcess process in this) process.OnCreate();
        }

        /// <summary>
        /// Procesa los datos de entrada
        /// </summary>
        /// <param name="data">Datos</param>
        public void ProcessData(DataOutputCollection outPut, IData data)
        {
            if (data == null) return;

            // Si es un enumerador, hay que pasar fila a fila al procesado 
            if (data is DataEnumerable)
            {
                // Recibe los datos enumerados
                DataEnumerable e = (DataEnumerable)data;

                // Lo ejecuta secuencial
                using (IEnumerator<object> enumerator = e.GetEnumerator())
                {
                    EEnumerableDataState state = EEnumerableDataState.Start;
                    bool last = !enumerator.MoveNext();
                    IData current;

                    while (!last)
                    {
                        current = new Data.DataObject(data.Source, enumerator.Current);

                        last = !enumerator.MoveNext();
                        if (last)
                        {
                            // Si es el último y era el primero, solo hay uno, sino, ha llegado al final
                            if (state == EEnumerableDataState.Start) state = EEnumerableDataState.OnlyOne;
                            else state = EEnumerableDataState.End;
                        }

                        // Ejecuta el procesado
                        if (UseParallel)
                        {
                            Parallel.ForEach<IDataProcess>(this, process => { process.ProcessData(current, state); });
                        }
                        else
                        {
                            foreach (IDataProcess process in this) process.ProcessData(current, state);
                        }

                        // Ejecuta la salida
                        if (outPut != null)
                        {
                            if (outPut.UseParallel)
                            {
                                Parallel.ForEach<IDataOutput>(outPut, process => { process.ProcessData(current, state); });
                            }
                            else
                            {
                                foreach (IDataOutput process in outPut) process.ProcessData(current, state);
                            }
                        }

                        state = EEnumerableDataState.Middle;
                    }
                }

                // Liberar los recursos del enumerado, posiblemente no se pueda reutilizar
                data.Dispose();
            }
            else
            {
                // Ejecuta el procesado
                if (UseParallel)
                {
                    Parallel.ForEach<IDataProcess>(this, process =>
                    {
                        process.ProcessData(data, EEnumerableDataState.NonEnumerable);
                    });
                }
                else
                {
                    foreach (IDataProcess process in this) process.ProcessData(data, EEnumerableDataState.NonEnumerable);
                }

                // Ejecuta la salida
                if (outPut != null)
                {
                    if (outPut.UseParallel)
                    {
                        Parallel.ForEach<IDataOutput>(outPut, process => { process.ProcessData(data, EEnumerableDataState.NonEnumerable); });
                    }
                    else
                    {
                        foreach (IDataOutput process in outPut) process.ProcessData(data, EEnumerableDataState.NonEnumerable);
                    }
                }
            }
        }
    }
}