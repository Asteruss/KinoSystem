﻿namespace KinoSystem.Models
{
    public class Hall
    {
        public int IdHall { get; set; }
        public string? NameHall { get; set; }
        public List<Row> Rows { get; set; }
        public Session? Session { get; set; }
    }
}
