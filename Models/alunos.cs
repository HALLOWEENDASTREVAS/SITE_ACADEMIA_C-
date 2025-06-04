using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AcademiaAPI.Models
{
    public class Aluno
    {
        [Key]
        public int IdAluno { get; set; }

        [Required]
        public string NomeAluno { get; set; }

        // Quantas aulas o aluno contratou
        public int AulasPlano { get; set; }

        // Quantas aulas ele já fez
        public int AulasFeitas { get; set; }

        public string PlanoAluno { get; set; }

        // Lista de Inscrições do aluno
        [JsonIgnore]
        public ICollection<Inscrito> Inscricoes { get; set; } = new List<Inscrito>();
    }
}
