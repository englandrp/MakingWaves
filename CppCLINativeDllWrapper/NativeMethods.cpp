/****************************** Module Header ******************************\
Module Name:  NativeMethods.cpp
Project:      CppCLINativeDllWrapper
Copyright (c) Microsoft Corporation.

The code in this file implements the C++/CLI wrapper classes that allow you 
to call from any .NET code to the functions exported by a native C++ DLL 
module.

  CSCallNativeDllWrapper/VBCallNativeDllWrapper (any .NET clients)
          -->
      CppCLINativeDllWrapper (this C++/CLI wrapper)
              -->
          CppDynamicLinkLibrary (a native C++ DLL module)

The NativeMethods class wraps the global functions exported by 
CppDynamicLinkLibrary.dll.

The interoperability features supported by Visual C++/CLI offer a 
particular advantage over other .NET languages when it comes to 
interoperating with native modules. Apart from the traditional explicit 
P/Invoke, C++/CLI allows implicit P/Invoke, also known as C++ Interop, or 
It Just Work (IJW), which mixes managed code and native code almost 
invisibly. The feature provides better type safety, easier coding, greater  
performance, and is more forgiving if the native API is modified. You can 
use the technology to build .NET wrappers for native C++ classes and 
functions if their source code is available, and allow any .NET clients to 
access the native C++ classes and functions through the wrappers.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


#pragma region Includes
// can we access the Kinect NUI functions here as well as c#...? apparently yes!

#include <iostream>
#include <fstream>
#include <sstream>

#include "NativeMethods.h"
using namespace CppCLINativeDllWrapper;

#include <msclr/marshal.h>
using namespace msclr::interop;
#pragma endregion

//blackbox includes
#include "skeleton.h"
#include "SeqDecTree.h"

#define VERBOSE true
//#define DISPLAY false
//#define XML_VERBOSE

#define NON_SIGN_NAME "-1"

// NOTE we're ignoring some skeleton joint data that we don't need - details below
//HIP_CENTER  KNEE_LEFT KNEE_RIGHT ANKLE_LEFT ANKLE_RIGHT FOOT_LEFT FOOT_RIGHT
/*
head = 0,
neck = 1,
torso = 2,
left_shoulder = 3,
left_elbow = 4,
left_hand = 5, 
left_hip = 6,
right_shoulder = 7,
right_elbow = 8,
right_hand = 9,
right_hip = 10,
left_wrist = 11,
right_wrist = 12,
skeleton_dim = 13
*/

char* body_GL[] ={"HEAD","SHOULDER_CENTER","SPINE","SHOULDER_LEFT","ELBOW_LEFT","HAND_LEFT","HIP_LEFT","SHOULDER_RIGHT","ELBOW_RIGHT","HAND_RIGHT","HIP_RIGHT","WRIST_LEFT", "WRIST_RIGHT"};

#define BODY_LOCS_SIZE 13

// this is where we store the skeleton data from frames to pass to the black box
vector<vector<vector<float> > > skeletonFrames; // vector containing stored FRAMES > JOINTS > POSITIONS ( x / y / z )
bool isRecording = true;

// this is what we store the results in...
vector<double> results;
//vector<string> modes;
int currentMode;
//vector<SeqDecTree*> classifiers;
SeqDecTree *theClassifier;
//struct for skeleton points - used for conversion to depth values (as with recorder)
struct SkeletonPoint {
	LONG   x;		///<	X position of skeleton joint
	LONG   y;		///<	Y position of skeleton joint
	USHORT  depth;	///<	depth data of skeleton joint

	SkeletonPoint() : x( 0 ), y( 0 ), depth( 0 ) {}
};


void NativeMethods::setSignPack(String ^classFile, String ^nameFile)
{
	string test1;
	string test2;
	MarshalString(nameFile,test1);
	MarshalString(classFile,test2);
	//test1 = "sptree_sd.200.30.2_feelings.name";
	//test2 = "sptree_sd.200.30.2_feelings.cls";
	cout << "<<< setSignPack >>> " << test1 << ", " << test2 << endl;
	theClassifier = new SeqDecTree(test1, test2); //possible memory leak??
}

void NativeMethods::MarshalString ( String ^ s, string& os ) {
   using namespace Runtime::InteropServices;
   const char* chars = 
      (const char*)(Marshal::StringToHGlobalAnsi(s)).ToPointer();
   os = chars;
   Marshal::FreeHGlobal(IntPtr((void*)chars));
}

void NativeMethods::startRecording()
{
	//clear vector
	skeletonFrames.clear();
	//set recording to true
	isRecording = true;
}

void NativeMethods::stopRecording()
{
	//check for sign...
	//getSignResults();
	skeletonFrames.clear();
	//set recording to false
	isRecording = false;
}

//	finds the most likely sign to have been made and returns the ID
int NativeMethods::getMostLikelySignResults()

{

	int result = -1;

	results.clear();
	double highestScore = 0;
	if(skeletonFrames.size() > 14){
		
		theClassifier->testExample(skeletonFrames, results);

		// this is what we store the results in...
		vector<double> results;
		theClassifier->testExample(skeletonFrames, results);
		//check for highest...
		for(unsigned int i(0); i<results.size(); i++){
			//is the sign correct?
			double thisResult = results[i];
			if(thisResult > 0.15){
				if(thisResult > highestScore){
					highestScore = thisResult;
					result = i;
				}
			}
			if( thisResult > 0.15) cout << theClassifier->mClassNames[i] << ' ' << thisResult << endl;
		}
	}

	stopRecording();

	//TODO return results? or return the likely sign? TBC...
	return result;

}


/*array<float>^*/ int NativeMethods::getSignResults( int theSignID)

{

	cout << " GET RESULTS... (no of frames recorded = " << skeletonFrames.size() << ")" << endl;
	int result = -1;
	//cout << " 188" << endl;
	results.clear();
	//cout << " 190" << endl;
	if(skeletonFrames.size() > 5){
		
		//cout << " GET RESULTS... " ;
		//cout << " 194" << endl;
		theClassifier->testExample(skeletonFrames, results);
		//cout << " 196" << endl;
		// this is what we store the results in...
		vector<double> results;

		theClassifier->testExample(skeletonFrames, results);
		//cout << " 201" << endl;
		// write out results to console...
		for(unsigned int i(0); i<results.size(); i++){
			//is the sign correct?
			//cout << " i " << i <<  endl;
			if(i == theSignID){
				double difficulty = 0.35;
				//cout << " 208" << endl;
				if(i < theClassifier->mClassNames.size()){
					if(theClassifier->mClassNames[i] == "angry") difficulty = 0.2;
					if(theClassifier->mClassNames[i] == "Angry") difficulty = 0.2;
					if(theClassifier->mClassNames[i] == "Go") difficulty = 0.2;
					if(theClassifier->mClassNames[i] == "Come") difficulty = 0.2;
					if(theClassifier->mClassNames[i] == "Carry") difficulty = 0.2;
					if(theClassifier->mClassNames[i] == "No") difficulty = 0.2;
					if(theClassifier->mClassNames[i] == "Yes") difficulty = 0.2;
					if(theClassifier->mClassNames[i] == "Careful") difficulty = 0.15;
					if(theClassifier->mClassNames[i] == "Fireexit") difficulty = 0.2;
					if(theClassifier->mClassNames[i] == "Alarm") difficulty = 0.11; // ?!
					if(theClassifier->mClassNames[i] == "Helmet") difficulty = 0.22;
					if(theClassifier->mClassNames[i] == "Boots") difficulty = 0.1; // ?!
					if(theClassifier->mClassNames[i] == "Clean") difficulty = 0.2;
					if(theClassifier->mClassNames[i] == "Door") difficulty = 0.1; // ?!
					if(theClassifier->mClassNames[i] == "Floor") difficulty = 0.15;
					if(theClassifier->mClassNames[i] == "Table") difficulty = 0.2;
					if(theClassifier->mClassNames[i] == "Dust") difficulty = 0.2;
					if(theClassifier->mClassNames[i] == "Sweep") difficulty = 0.15;
					if(theClassifier->mClassNames[i] == "Polish") difficulty = 0.15;
				} else {
					if(theSignID == 6){
						cout << " Toilet? " << results[i] << endl;
						difficulty = 0.2;
					}
				}
				//cout << " 210" << endl;
				result = (results[i] >= difficulty) ? 1 : 0;
				//cout << " 212" << endl;
			}
			//cout << i << " " << theClassifier->mClassNames.size() << " " << results.size() << endl;
			if( i < theClassifier->mClassNames.size() && i < results.size()) {
				if(results[i] > 0.1) cout << theClassifier->mClassNames[i] << ' ' << results[i] << endl;
			} else {
				//cout << "ooer" << endl;
			}
			//cout << " k then " <<  endl;
		}
		//cout << " ok dokey " <<  endl;
		//cout << " 213" << endl;


	} else {
	//	cout << "Sequence too short!";
	}
	//cout << " 219" << endl;
	stopRecording();

	//cout << " 222" << endl;
	//TODO return results? or return the likely sign? TBC...
	return result;

}


int NativeMethods::processSkeleton(array<float> ^ floatArray)
{
	int theInt = 0;

	if(isRecording){
		pin_ptr<float> testarray = &floatArray[0];
		//float anArray[] = floatArray[0];

		//theInt = (int)::processSkeleton(testarray);

		// black box stuff
		int posInt = 0;
	
		vector< vector<float> > joints;
		for(unsigned int i(1); i<40; i+=3){
			//add x / y / z ( 0 / 1 / 2 index) to pos vector
			vector<float> pos;
			//can't remember why we're passing the 4th value below...? Has to be Vec4 though for NuiTransformSkeletonToDepthImage function to work
			Vector4 vPoint = { floatArray[i], floatArray[i+1], floatArray[i+2], 1.0 };

			SkeletonPoint p; //to hold the converted points
		
			//transform the position values to depth values (so it's the same as data from the recorder)
			NuiTransformSkeletonToDepthImage( vPoint, &p.x, &p.y, &p.depth );

			pos.push_back( p.x ) ;		//x
			pos.push_back( p.y ) ;		//y
			pos.push_back( p.depth ) ;	//z

			//debugging code only
			//if(i==1) {
				//cout << "head pos " << p.x << " " << p.y << " " << p.depth << endl;
			//}

			//add to joints vector
			joints.push_back(pos);
			posInt = 0;
			pos.clear();
			//delete pos;
		}
		//add the whole caboodle to frames vector
		skeletonFrames.push_back( joints );

		joints.clear();
		//delete &joints;
	}
	
	//cout << "Frames " << skeletonFrames.size() << endl;

	if(skeletonFrames.size() > 100){ // max size of sign - send the data anyway...
		//stopRecording();
	}

	//theInt = ::doSomething2(0);
	return theInt;

}

