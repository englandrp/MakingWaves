
#pragma once

#include <memory>
#include <tuple>

#include <Windows.h>
#include <NuiApi.h>

// Include the header file of the native C++ module.
//#include "CppDynamicLinkLibrary.h"
//#include "kinect/nui/Kinect.h" // this should be before ofMain.h
//#include "kinect/nui/ImageFrame.h" // for VideoFrame and DepthFrame
#include "SeqDecTree.h"

using namespace System;
using namespace System::Runtime::InteropServices;


namespace CppCLINativeDllWrapper
{
    public ref class NativeMethods
    {
    public:
		static void setSignPack(String ^classFile, String ^nameFile);
		static int processSkeleton(array<float>^ floatArray);
		//static void setResolution(array<int>^ resArray);
		//static void setBoneCount(int theBoneCount);
		
static void MarshalString ( String ^ s, string& os ) ;
		static void startRecording();
		static void stopRecording();
		static /*array<float>^*/ int getSignResults(int theSignID);
		static int  getMostLikelySignResults();
		//static void processImage(array<unsigned char>^ charArray);
    };
}