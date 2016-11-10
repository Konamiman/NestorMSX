<Query Kind="Statements" />

var lines = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "../InstructionTemplatesSource.txt"));

var items =
	lines
	.Select(l => l.Trim().Replace("   ", " ").Replace("  ", " "))
	.Where(l => l != "" && !l.StartsWith(";"))
	.Select(l => { var tokens = l.Split(';').Select(t => t.Trim()).ToArray(); return tokens; })
	.Where(t => t[1].StartsWith("DD CB"))
	.Select(t => new Tuple<string, string[], byte[]>(t[0], t[1].Split(' '), t[1].Split(' ').Select(x => x=="DIS" || x.StartsWith("N") ? (byte)0 : Byte.Parse(x, System.Globalization.NumberStyles.AllowHexSpecifier )).ToArray()))
	.OrderBy(t => t.Item2[1])
	.ToDictionary(t => (byte)t.Item3[3], t => t)
	;

((0x40 >> 3) & 3).Dump();
((0x48 >> 3) & 3).Dump();
((0x50 >> 3) & 3).Dump();
((0x58 >> 3) & 3).Dump();

items.OrderBy(i => i.Key).Dump();