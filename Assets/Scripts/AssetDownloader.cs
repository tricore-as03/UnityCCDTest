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
    /// ダウンロード
    /// </summary>
    private async UniTask Download()
    {
        // ダウンロードサイズを取得
        var getDownloadSizeHandle = Addressables.GetDownloadSizeAsync("PlayOnDownloadAssets");
        await getDownloadSizeHandle.Task;

        // ダウンロードが必要だったらダウンロード
        if (getDownloadSizeHandle.Result > 0)
        {
            // ダウンロード
            // 内部的にはLoadAssetAsyncしてReleaseしてるだけ
            var downloadDependencies = Addressables.DownloadDependenciesAsync("PlayOnDownloadAssets");
            await downloadDependencies.Task;
        }

        var material = await assetReference.LoadAssetAsync<Material>();
    }
}
