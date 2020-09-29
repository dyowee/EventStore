using System;
using System.Collections.Generic;
using EventStore.Core.Index;

namespace EventStore.Core.Tests.Index {
	public static class IndexMapExtensions {
		public static MergeResult AddAndMergePTable(
			this IndexMap indexMap,
			PTable tableToAdd,
			int prepareCheckpoint,
			int commitCheckpoint,
			Func<string, ulong, ulong> upgradeHash,
			Func<IndexEntry, bool> existsAt,
			Func<IndexEntry, Tuple<string, bool>> recordExistsAt,
			IIndexFilenameProvider filenameProvider,
			byte version,
			int indexCacheDepth = 16,
			bool skipIndexVerify = false) {

			var addResult = indexMap.AddPTable(tableToAdd, prepareCheckpoint, commitCheckpoint);
			if (addResult.CanMergeAny) {
				var toDelete = new List<PTable>();
				MergeResult mergeResult;
				IndexMap curMap = addResult.NewMap;
				do {
					mergeResult = curMap.TryMergeOneLevel(
						upgradeHash,
						existsAt,
						recordExistsAt,
						filenameProvider,
						version,
						indexCacheDepth,
						skipIndexVerify
					);

					curMap = mergeResult.MergedMap;
					toDelete.AddRange(mergeResult.ToDelete);
				} while (mergeResult.CanMergeAny);

				return new MergeResult(curMap, toDelete, true, false);
			}
			return new MergeResult(addResult.NewMap, new List<PTable>(), false, false);
		}
	}
}
