using KinoSystem.Models.Utilities;

namespace KinoSystem.Models
{
	public class AddSession
	{
		public DateTime Time { get; set; }
		public int IdMovie { get; set; }
		public int Price { get; set; }
		public HallType HallType { get; set; }

	}
}
