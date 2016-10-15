<Query Kind="Statements" />

//The resulting file includes all standard documented and undocumented instructions,
//but does NOT include mirrors and undefined instructions

var lines = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "InstructionTemplatesSourceOriginal.txt"));

var toIgnore = new[] {
		"jr po", "jr pe", "jr p", "jr m",
		"ldi ", "ldd ", "stop", "sub hl,sp"
	};

var regexToIgnore = new Regex(@"(ld|sub) (bc|de|hl|ix|iy),\(?(bc|de|hl|ix|iy)\)?");

var validRsts = new[] { 0, 8, 16, 24, 32, 40, 48, 56}.Select(l => l.ToString());

lines = lines
	.Select(l => l.Trim().Replace("   ", " ").Replace("  ", " "))
	.Where(l => l!="" && !l.StartsWith(";"))
	.Where(l => !l.Contains("NDIS"))
	.Where(l => {
		var parts = l.Split(';')[1].Trim().Split(' ');
		return parts.Length <= 4;
	})
	.Where(l => !toIgnore.Any(ti => l.Split(';')[0].Trim().Contains(ti)))
	.Where(l => !regexToIgnore.IsMatch(l))
	.Where(l => !l.StartsWith("rst") || validRsts.Any(r => l.Contains(r)))
	.Where(l => (l.Contains("(ix)") || l.Contains("(iy)")) && !l.Contains("00"))
	.ToArray();
	
lines.Dump();

File.WriteAllLines(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "InstructionTemplatesSource.txt"), lines);