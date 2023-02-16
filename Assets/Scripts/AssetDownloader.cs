using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AssetDownloader : MonoBehaviour
{
    [SerializeField]
    private AssetReference assetReference = default;

    // Start is called before the first frame update
    void Start()
    {
        Download().Forget();
    }

    /// <summary>
    /// �_�E�����[�h
    /// </summary>
    private async UniTask Download()
    {
        // �_�E�����[�h�T�C�Y���擾
        var getDownloadSizeHandle = Addressables.GetDownloadSizeAsync("PlayOnDownloadAssets");
        await getDownloadSizeHandle.Task;
        
        // �_�E�����[�h���K�v��������_�E�����[�h
        //if (getDownloadSizeHandle.Result > 0)
        //{
            // �_�E�����[�h
        //await downloadDependencies.Task;
        //}

        await Addressables.DownloadDependenciesAsync("PlayOnDownloadAssets");
        await assetReference.LoadAssetAsync<GameObject>();
        await assetReference.InstantiateAsync();
    }
}
