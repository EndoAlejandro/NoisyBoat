using UnityEngine;
using UnityEngine.Rendering;

namespace FlatKit {
public static class RendererFeatureUtils {
    public static void SetKeyword(Material material, string keyword, bool enabled) {
        if (material.shader != null) {
            // In newer Unity versions, constructing a LocalKeyword directly from a name that is not part
            // of the shader's keyword space logs an error. FindKeyword looks it up silently instead,
            // returning an invalid keyword when the shader doesn't declare it.
            var localKeyword = material.shader.keywordSpace.FindKeyword(keyword);
            if (localKeyword.isValid) {
                material.SetKeyword(localKeyword, enabled);
            }
        } else {
            if (enabled) {
                material.EnableKeyword(keyword);
            } else {
                material.DisableKeyword(keyword);
            }
        }
    }
}
}