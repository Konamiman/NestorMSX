<Query Kind="Statements" />

var lines = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "../InstructionTemplatesSource.txt"));

var items =
	lines
	.Select(l => l.Trim().Replace("   ", " ").Replace("  ", " "))
	.Where(l => l != "" && !l.StartsWith(";"))
	.Select(l => { var tokens = l.Split(';').Select(t => t.Trim()).ToArray(); return tokens; })
	.Where(t => !t[1].StartsWith("CB") && !t[1].StartsWith("ED") && !t[1].StartsWith("DD") && !t[1].StartsWith("FD"))
	.Select(t => new Tuple<string, string[], byte[]>(t[0], t[1].Split(' '), t[1].Split(' ').Select(x => x=="DIS" || x.StartsWith("N") ? (byte)0 : Byte.Parse(x, System.Globalization.NumberStyles.AllowHexSpecifier )).ToArray()))
	.OrderBy(t => t.Item2[0])
	.ToArray()
	;

var nullTuple = new[] { new Tuple<string, string[], byte[]>(null, null, null)};

items = items
	.Take(0xCB)
	.Concat(nullTuple)
	.Concat(items.Skip(0xCB).Take(0xDD-0xCB-1))
	.Concat(nullTuple)
	.Concat(items.Skip(0xDD-1).Take(0xED-0xDD-1))
	.Concat(nullTuple)
	.Concat(items.Skip(0xED-2).Take(0xFD-0xED-1))
	.Concat(nullTuple)
	.Concat(items.Skip(0xFD-3).Take(0xFF-0xFD))
	.ToArray()
	;

items.Dump();