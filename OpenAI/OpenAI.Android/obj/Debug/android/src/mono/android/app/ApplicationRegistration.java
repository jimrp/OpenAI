package mono.android.app;

public class ApplicationRegistration {

	public static void registerApplications ()
	{
				// Application and Instrumentation ACWs must be registered first.
		mono.android.Runtime.register ("OpenAI.Droid.MainApplication, OpenAI.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", crc64ae80cc097a76c553.MainApplication.class, crc64ae80cc097a76c553.MainApplication.__md_methods);
		
	}
}
