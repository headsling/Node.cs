using System;
using Manos.Http;
using Manos.Collections;
using System.Text;
namespace Manos
{
	public class HttpBufferedBodyHandler : IHttpBodyHandler {

		private StringBuilder builder;

		public void HandleData (HttpEntity entity, ByteBuffer data, int pos, int len)
		{
			if (builder == null)
				builder = new StringBuilder ();

			string str = entity.ContentEncoding.GetString (data.Bytes, pos, len);
			builder.Append (str);
		}

		public void Finish (HttpEntity entity)
		{
			entity.PostBody = builder.ToString ();
		}
	}
}

