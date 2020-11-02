﻿using DFC.Composite.Shell.Models;
using DFC.Composite.Shell.Models.Common;
using DFC.Composite.Shell.Services.AssetLocationAndVersion;
using Microsoft.Extensions.Configuration;
using System;

namespace DFC.Composite.Shell.Utilities
{
    public class VersionedFiles : IVersionedFiles
    {
        public VersionedFiles(IConfiguration configuration, IAssetLocationAndVersionService assetLocationAndVersionService, WebchatOptions webchatOptions)
        {
            _ = webchatOptions ?? throw new ArgumentNullException(nameof(webchatOptions));

            var brandingAssetsCdn = configuration.GetValue<string>("BrandingAssetsCdn");
            var brandingAssetsFolder = $"{brandingAssetsCdn}/{Constants.NationalCareersToolkit}";

            VersionedPathForMainMinCss = assetLocationAndVersionService?.GetCdnAssetFileAndVersion($"{brandingAssetsFolder}/css/all.min.css");
            VersionedPathForGovukMinCss = assetLocationAndVersionService?.GetCdnAssetFileAndVersion($"{brandingAssetsFolder}/css/govuk.min.css");
            VersionedPathForAllIe8Css = assetLocationAndVersionService?.GetCdnAssetFileAndVersion($"{brandingAssetsFolder}/css/all-ie8.css");

            VersionedPathForJQueryBundleMinJs = assetLocationAndVersionService?.GetCdnAssetFileAndVersion($"{brandingAssetsFolder}/js/jquerybundle.min.js");
            VersionedPathForAllMinJs = assetLocationAndVersionService?.GetCdnAssetFileAndVersion($"{brandingAssetsFolder}/js/all.min.js");
            VersionedPathForDfcDigitalMinJs = assetLocationAndVersionService?.GetCdnAssetFileAndVersion($"{brandingAssetsFolder}/js/dfcdigital.min.js");
            VersionedPathForCompUiMinJs = assetLocationAndVersionService?.GetCdnAssetFileAndVersion($"{brandingAssetsFolder}/js/compui.min.js");

            if (webchatOptions.Enabled)
            {
                WebchatEnabled = webchatOptions.Enabled;

                VersionedPathForWebChatJs = assetLocationAndVersionService?.GetCdnAssetFileAndVersion(webchatOptions.ScriptUrl);
            }
        }

        public string VersionedPathForMainMinCss { get; }

        public string VersionedPathForGovukMinCss { get; }

        public string VersionedPathForAllIe8Css { get; }

        public string VersionedPathForJQueryBundleMinJs { get; }

        public string VersionedPathForAllMinJs { get; }

        public string VersionedPathForDfcDigitalMinJs { get; }

        public string VersionedPathForCompUiMinJs { get; }

        public string VersionedPathForWebChatJs { get; }

        public bool WebchatEnabled { get; }
    }
}
