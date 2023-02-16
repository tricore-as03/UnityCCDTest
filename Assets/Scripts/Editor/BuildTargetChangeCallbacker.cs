using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildTargetChangeCallbacker : UnityEditor.Build.IActiveBuildTargetChanged
{
    // interface ÇÃìsçáè„ÅAïKóv
    public int callbackOrder { get { return 0; } }

    public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
    {
        Debug.Log($"Change Platform OS '{newTarget}'");
        Debug.Log(CcdManager.EnvironmentName);
    }
}
