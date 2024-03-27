using System;
namespace OpenAI.Services
{
	public interface ISpeechToText
	{
        void StartSpeechToText();
        void StopSpeechToText();
    }
}

