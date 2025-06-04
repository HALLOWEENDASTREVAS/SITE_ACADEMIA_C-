using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AcademiaAPI.Models
{
    public class Inscrito
    {
        // Chave composta (IdAluno, IdAula)
        [Key, Column(Order = 0)]
        public int IdAluno { get; set; }

        [Key, Column(Order = 1)]
        public int IdAula { get; set; }

        [JsonIgnore]
        public Aluno? Aluno { get; set; }

        [JsonIgnore]
        public Aula? Aula { get; set; }
    }
}
