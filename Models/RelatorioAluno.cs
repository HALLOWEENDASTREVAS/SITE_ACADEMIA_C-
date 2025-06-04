using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcademiaAPI.Data;
using AcademiaAPI.Models;

public class RelatorioAluno
{
    public int IdAluno { get; set; }
    public int TotalAulasNoMes { get; set; }
    public List<RelatorioTipoAula> TiposAulaMaisFrequente { get; set; }
}

public class RelatorioTipoAula
{
    public string TipoAula { get; set; }
    public int Quantidade { get; set; }
}