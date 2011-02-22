using System;
using System.Reflection;

namespace NodeCS.Exceptions
{
	public static class Preconditions
	{

        public static void IsNotNull( object value, string message, params string[] paramList )
        {
            IsNotNull<Exception>( value, message, paramList );
        }

        public static void IsNotNull<T>( object value, string message, params string[] paramList ) where T : Exception
        {
            if( value != null ) return;

            throwException( typeof(T), message, paramList);
        }

        public static void IsNull( object value, string message, params string[] paramList )
        {
            IsNull<Exception>( value, message, paramList );
        }

        public static void IsNull<T>( object value, string message, params string[] paramList ) where T : Exception
        {
            if( value != null ) return;

            throwException( typeof(T), message, paramList);
        }

        public static void IsFalse( bool value, string message, params string[] paramList )
        {
            IsTrue<Exception>( !value, message, paramList );
        }

        public static void IsFalse<T>( bool value, string message, params string[] paramList ) where T : Exception
        {
            IsTrue<T>( !value, message, paramList );
        }

		public static void IsTrue( bool value, string message, params string[] paramList ) 
		{
			IsTrue<Exception>( value, message, paramList ); 
		}
		
		public static void IsTrue<T>( bool value, string message, params string[] paramList ) where T : Exception
		{
			if( value ) return;
			
			throwException( typeof(T), message, paramList); 
		}

		static void throwException (Type exceptionType, string message, string[] paramList)
		{
			ConstructorInfo constructorInfo = exceptionType.GetConstructor ( new[] { typeof(string) });
			
			Exception ex  = (Exception)constructorInfo.Invoke (new object[] { message });
			throw ex;
		}

		
	}
}

