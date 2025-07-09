// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using Editor.Content;
using System;

namespace Editor.Editors
{
	enum AssetEditorState
	{
		Done = 0,
		Importing,
		Processing,
		Loading,
		Saving,
	}
	
    interface IAssetEditor
    {
		AssetEditorState State { get; }
		Guid AssetGuid { get; }
		Asset Asset { get; }
		
        void SetAsset(AssetInfo info);
    }
}
