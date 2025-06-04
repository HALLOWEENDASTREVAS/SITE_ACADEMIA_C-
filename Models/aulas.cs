using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace AcademiaAPI.Models
{
    public class Aula
    {
        [Key]
        public int IdAula { get; set; }

        [Required]
        public DateTime DataAula { get; set; }

        public int Capacidade { get; set; }

        public int Confirmados { get; set; }

        public string TipoAula { get; set; }

        // Lista de Inscrições para a aula
        [JsonIgnore]
        public ICollection<Inscrito> Inscricoes { get; set; } = new List<Inscrito>();
    }
}
