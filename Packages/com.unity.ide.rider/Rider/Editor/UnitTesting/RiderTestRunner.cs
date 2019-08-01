using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Packages.Rider.Editor.UnitTesting
{
  public static class RiderTestRunner
  {
    private static readonly TestsCallback Callback = ScriptableObject.CreateInstance<TestsCallback>();
    
    [UsedImplicitly]
    public static void RunTests(int testMode, string[] assemblyNames, string[] testNames, string[] categoryNames, string[] groupNames, int? buildTarget)
    {
      CallbackData.instance.isRider = true;
            
      var api = ScriptableObject.CreateInstance<TestRunnerApi>();
      api.Execute(new ExecutionSettings()
      {
        filters = new []{
        new Filter()
        {
         assemblyNames = assemblyNames,
         testNames = testNames,
         categoryNames = categoryNames,
         groupNames = groupNames 
         , targetPlatform = (BuildTarget?)buildTarget // This can be used to set the target platform when running playmode tests.
         , testMode = (TestMode)testMode
        }
        }
      });
      
      api.UnregisterCallbacks(Callback); // avoid multiple registrations
      api.RegisterCallbacks(Callback); // This can be used to receive information about when the test suite and individual tests starts and stops. Provide this with a scriptable object implementing ICallbacks
    }
  }
}