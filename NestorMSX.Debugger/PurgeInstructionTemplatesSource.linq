<Query Kind="Statements" />

var lines = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "InstructionTemplatesSourceOriginal.txt"));

var toIgnore = new[] {
		"jr po", "jr pe", "jr p", "jr m",
		"ldi ", "ldd "
	};

lines = lines
	.Select(l => l.Trim().Replace("   ", " ").Replace("  ", " "))
	.Where(l => l!="" && !l.StartsWith(";"))
	.Where(l => !l.Contains("NDIS"))
	.Where(l => {
		var parts = l.Split(';')[1].Trim().Split(' ');
		return parts.Length <= 4;
	})
	.Where(l => !toIgnore.Any(ti => l.Split(';')[0].Trim().Contains(ti)))
	.ToArray();
	
lines.Dump();

File.WriteAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "InstructionTemplatesSource.txt"), lines);