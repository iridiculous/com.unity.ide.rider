using JetBrains.Annotations;
using UnityEngine;
#if TEST_FRAMEWORK
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
#else
using System;
#endif

namespace Packages.Rider.Editor.UnitTesting
{
  [UsedImplicitly] // from Rider Unity plugin
  public static class RiderTestRunner
  {
#if TEST_FRAMEWORK
    private static readonly TestsCallback Callback = ScriptableObject.CreateInstance<TestsCallback>();
#endif
    
    [UsedImplicitly]
    public static void RunTestsWithSyncCallbacks(string sessionId, int testMode, string[] assemblyNames, 
      string[] testNames, string[] categoryNames, string[] groupNames, int? buildTarget,  
      string callbacksHandlerCodeBase, string callbacksHandlerTypeName, string[] callbacksHandlerDependencies)
    {
#if !TEST_FRAMEWORK
      Debug.LogError("Update Test Framework package to v.1.1.1+ to run tests from Rider.");
      throw new NotSupportedException("Incompatible `Test Framework` package in Unity. Update to v.1.1.1+");
#else
      SyncTestRunEventsHandler.instance.InitRun(sessionId, callbacksHandlerCodeBase, callbacksHandlerTypeName, callbacksHandlerDependencies);
      RunTests(testMode, assemblyNames, testNames, categoryNames, groupNames, buildTarget);
#endif      
    }
    
    [UsedImplicitly]
    public static void RunTests(int testMode, string[] assemblyNames, string[] testNames, string[] categoryNames, string[] groupNames, int? buildTarget)
    {
#if !TEST_FRAMEWORK
      Debug.LogError("Update Test Framework package to v.1.1.1+ to run tests from Rider.");
      throw new NotSupportedException("Incompatible `Test Framework` package in Unity. Update to v.1.1.1+");
#else
      CallbackData.instance.isRider = true;
            
      var api = ScriptableObject.CreateInstance<TestRunnerApi>();
      var settings = new ExecutionSettings();
      var filter = new Filter
      {
        assemblyNames = assemblyNames,
        testNames = testNames,
        categoryNames = categoryNames,
        groupNames = groupNames,
        targetPlatform = (BuildTarget?) buildTarget
      };
      
      if (testMode > 0) // for future use - test-framework would allow running both Edit and Play test at once
        filter.testMode = (TestMode) testMode;
      
      settings.filters = new []{
        filter
      };
      api.Execute(settings);
      
      api.UnregisterCallbacks(Callback); // avoid multiple registrations
      api.RegisterCallbacks(Callback); // This can be used to receive information about when the test suite and individual tests starts and stops. Provide this with a scriptable object implementing ICallbacks
#endif
    }
  }
}