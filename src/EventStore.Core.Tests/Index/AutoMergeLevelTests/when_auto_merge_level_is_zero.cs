﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace EventStore.Core.Tests.Index.AutoMergeLevelTests {
	[TestFixture]
	public class when_auto_merge_level_is_zero : when_max_auto_merge_level_is_set {
		public when_auto_merge_level_is_zero() : base(0) {
		}

		[Test]
		public void manual_merge_should_merge_all_tables() {
			AddTables(11);
			_result = _result.MergedMap.TryManualMerge(
				UpgradeHash, ExistsAt, RecordExistsAt,
				_fileNameProvider, _ptableVersion);
			Assert.AreEqual(1, _result.MergedMap.InOrder().Count());
		}
	}
}
