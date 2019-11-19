using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.PackageManager.ValidationSuite;

namespace RiderEditor
{
    public class Package
    {
        [Test]
        public void Validate()
        {
            const string package = "com.unity.ide.rider@1.1.3-preview.1";
            var result = ValidationSuite.ValidatePackage(package, ValidationType.LocalDevelopment);
            Debug.Log(ValidationSuite.GetValidationSuiteReport(package));
            Assert.True(result);
        }
    }
}
