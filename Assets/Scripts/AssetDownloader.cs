using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (getDownloadSizeHandle.Result > 0)
        {
            // �_�E�����[�h
            // �����I�ɂ�LoadAssetAsync����Release���Ă邾��
            var downloadDependencies = Addressables.DownloadDependenciesAsync("PlayOnDownloadAssets");
            await downloadDependencies.Task;
        }

        var material = await assetReference.LoadAssetAsync<Material>();
    }
}
