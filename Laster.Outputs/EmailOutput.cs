﻿using Laster.Core.Data;
using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Laster.Outputs
{
    public class EmailOutput : IDataOutput
    {
        /// <summary>
        /// Formato
        /// </summary>
        [Category("Attachment")]
        public SerializationHelper.EFormat Format { get; set; }
        /// <summary>
        /// Codificación
        /// </summary>
        [Category("Attachment")]
        public SerializationHelper.EEncoding Encoding { get; set; }
        /// <summary>
        /// AttachmentName
        /// </summary>
        [Category("Attachment")]
        public string AttachmentName { get; set; }

        #region SMTP
        /// <summary>
        /// Host
        /// </summary>
        [Category("Connection")]
        public string SmtpHost { get; set; }
        /// <summary>
        /// Port
        /// </summary>
        [Category("Connection")]
        public int SmtpPort { get; set; }
        /// <summary>
        /// Activar SSL
        /// </summary>
        [Category("Connection")]
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Host
        /// </summary>
        [Category("Credentials")]
        public string User { get; set; }
        /// <summary>
        /// Host
        /// </summary>
        [Category("Credentials")]
        public string Password { get; set; }

        /// <summary>
        /// From
        /// </summary>
        [Category("Email")]
        public string From { get; set; }
        /// <summary>
        /// To
        /// </summary>
        [Category("Email")]
        public string[] To { get; set; }
        /// <summary>
        /// BCC
        /// </summary>
        [Category("Email")]
        public string[] BCC { get; set; }
        /// <summary>
        /// Subject
        /// </summary>
        [Category("Email")]
        public string Subject { get; set; }
        /// <summary>
        /// Body
        /// </summary>
        [Category("Email")]
        [EditorAttribute(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Body { get; set; }
        /// <summary>
        /// SendEmpty
        /// </summary>
        [Category("Email")]
        public bool SendEmpty { get; set; }
        /// <summary>
        /// SendAsync
        /// </summary>
        [Category("Email")]
        public bool SendAsync { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public EmailOutput()
        {
            Format = SerializationHelper.EFormat.Json;
            Encoding = SerializationHelper.EEncoding.UTF8;
        }

        SmtpClient _Smtp;

        public override void OnCreate()
        {
            base.OnCreate();

            _Smtp = new SmtpClient(SmtpHost, SmtpPort);
            _Smtp.Credentials = new NetworkCredential(User, Password);
            _Smtp.EnableSsl = EnableSsl;
        }

        public override void Dispose()
        {
            if (_Smtp != null)
            {
                _Smtp.Dispose();
                _Smtp = null;
            }
            base.Dispose();
        }

        protected override void OnProcessData(IData data, EEnumerableDataState state)
        {
            if (data == null || data is DataEmpty)
            {
                if (!SendEmpty) return;
            }

            // Formato del archivo
            byte[] cad = SerializationHelper.Serialize(data.GetInternalObject(), Encoding, Format);

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(From);
            msg.Subject = Subject;
            msg.Body = Body;

            foreach (string t in To) msg.To.Add(t);
            foreach (string t in BCC) msg.Bcc.Add(t);

            using (MemoryStream ms = new MemoryStream(cad))
                msg.Attachments.Add(new Attachment(ms, AttachmentName, SerializationHelper.GetMimeType(Format)));

            if (SendAsync) _Smtp.SendAsync(msg, null);
            else _Smtp.Send(msg);
        }
    }
}