import clr
clr.AddReferenceByName("UtilityExtensions")
from IronPythonUtilities import ConfigurationProxy

def override(filename):
    proxy = ConfigurationProxy(filename)
    return proxy.InjectToConfigurationManager()