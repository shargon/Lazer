﻿using Laster.Core.Classes.RaiseMode;
using Laster.Core.Data;
using Laster.Core.Interfaces;
using System;
using TweetSharp;

namespace Laster.Inputs.Twitter
{
    public class TwitterSearchInput : ITwitterInput
    {
        /// <summary>
        /// Incluir entidades
        /// </summary>
        public bool IncludeEntities { get; set; }
        /// <summary>
        /// Cantidad
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Posición geográfica
        /// </summary>
        public TwitterGeoLocationSearch GeoCode { get; set; }
        /// <summary>
        /// Tipo de resultado
        /// </summary>
        public TwitterSearchResultType? ResultType { get; set; }
        /// <summary>
        /// Idioma
        /// </summary>
        public string Lang { get; set; }
        /// <summary>
        /// Local
        /// </summary>
        public string Locale { get; set; }
        /// <summary>
        /// Consulta
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public TwitterSearchInput(TimeSpan interval) : base(interval) { Init(); }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="trigger">Trigger</param>
        public TwitterSearchInput(DataInputTrigger trigger) : base(trigger) { Init(); }
        /// <summary>
        /// Inicializa
        /// </summary>
        void Init()
        {
            Count = 10;
            IncludeEntities = true;
            ResultType = TwitterSearchResultType.Mixed;
            GeoCode = null;
        }

        protected override IData OnGetData()
        {
            TwitterSearchResult tweets = Service.Search(new SearchOptions()
            {
                Geocode = GeoCode,
                Lang = Lang,
                Locale = Locale,
                Q = Query,
                Resulttype = ResultType,
                IncludeEntities = IncludeEntities,
                Count = Count,
                //SinceId = SinceId,
            });

            return new IEnumerableData(this, tweets.Statuses);
        }
    }
}