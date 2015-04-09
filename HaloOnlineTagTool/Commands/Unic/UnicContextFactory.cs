﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloOnlineTagTool.TagStructures;

namespace HaloOnlineTagTool.Commands.Unic
{
	static class UnicContextFactory
	{
		public static CommandContext Create(CommandContext parent, FileInfo fileInfo, TagCache cache, HaloTag tag,
			MultilingualUnicodeStringList unic)
		{
			var context = new CommandContext(parent, string.Format("{0:X8}.unic", tag.Index));
			context.AddCommand(new UnicListCommand(unic));
			return context;
		}
	}
}