using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Text;
using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEditor.AddressableAssets.Settings;
using Unity.Services.Core;

public class CCDEditorWindow : EditorWindow
{
    private string ProjectId => CloudProjectSettings.projectId;
    private string Ucd => $"{Application.dataPath}/../ucd.exe";
    private string AssetBundleRootPath => $"{Application.dataPath}/../ServerData";

    // NOTE : Jsonで外部管理すべき
    private string apiKey = "8546100b39bdfabe43ce478e81e9653c";
    
    // NOTE : 本来はリストかなんかで管理すべき
    private const string DevelopmentEnvId = "14e5d2ca-37dc-47cd-9178-849d275f6dca";
    private const string ProductionEnvId = "cfac20d5-02ae-487a-bd0e-bb48bb23dc82";
    private const string DevelopmentAndroidBucketId = "19f9dfba-c81b-41c9-93c9-0db311ccf233";
    private const string ProductionAndroidBucketId = "8942a71c-7ebe-4a79-9094-4ba73f9e976a";

    private StringBuilder logs = new StringBuilder();
    private Vector2 logScroll = Vector2.zero;

    [MenuItem("Editor/CCD Editor")]
    private static void Create()
    {
        // 生成
        GetWindow<CCDEditorWindow>("CCD Editor");
    }

    private void OnGUI()
    {
        using (var scope = new GUILayout.VerticalScope(GUI.skin.box))
        {
            SwitchEnvironmentAndPlatform();
            //AddressableAssetUtility
        }

        using (var scope = new GUILayout.VerticalScope(GUI.skin.box))
        {
            OnGUIForUploadAssets();
        }
    }

    private void SwitchEnvironmentAndPlatform()
    {

    }

    private void OnGUIForUploadAssets()
    {
        EditorGUILayout.LabelField("CCDにアップロード", EditorStyles.boldLabel);
        if (GUILayout.Button("アップロード"))
        {
            UploadAssets().Forget();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("実行ログ", EditorStyles.boldLabel);
        ShowLogs();
    }

    private async UniTask UploadAssets()
    {
        logs.Clear();

        // CLI (Old)

        var commands = new string[]
        {
            $"auth login {apiKey}",
            $"config set environment {DevelopmentEnvId} --project={ProjectId}",
            $"config set bucket {DevelopmentAndroidBucketId}",
            $"entries sync {AssetBundleRootPath}/{BuildTarget.StandaloneWindows64}",
            $"releases create",
            $"auth logout {apiKey}",
        }
        .ToUniTaskAsyncEnumerable();
        
        var processStartInfo = new ProcessStartInfo(Ucd);
        processStartInfo.UseShellExecute = false;
        processStartInfo.RedirectStandardOutput = true;
        processStartInfo.RedirectStandardError = true;
        processStartInfo.RedirectStandardInput = false;
        processStartInfo.CreateNoWindow = true;
        
        await foreach (var command in commands)
        {
            processStartInfo.Arguments = command;
        
            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.OutputDataReceived += OnReceiveUcdData;
                process.ErrorDataReceived += OnReceiveUcdError;
        
                process.Start();
                process.BeginOutputReadLine();
        
                await UniTask.WaitUntil(() => process.HasExited);
        
                process.OutputDataReceived -= OnReceiveUcdData;
                process.ErrorDataReceived -= OnReceiveUcdError;
            }
        }

        // 対象のEnvironmentをセット

    }

    private void ShowLogs()
    {
        logScroll = EditorGUILayout.BeginScrollView(logScroll);
        foreach (var logLine in logs.ToString().Split("\n"))
        {
            EditorGUILayout.LabelField(logLine);
        }
        EditorGUILayout.EndScrollView();
    }

    private void OnReceiveUcdData(object sender, DataReceivedEventArgs e)
    {
        logs.AppendLine(e.Data);
    }

    private void OnReceiveUcdError(object sender, DataReceivedEventArgs e)
    {
        logs.AppendLine(e.Data);
    }
}
