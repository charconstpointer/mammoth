using System.Collections.Generic;
using System.Linq;
using Mammoth.Worker.DTO;

namespace Mammoth.Worker.Extensions
{
    public static class ScheduleExtensions
    {
        public static IEnumerable<ProgramDto> AsDto(this IEnumerable<Program> programs)
        {
            return programs.Select(AsDto).SelectMany(p=>p);
        }

        public static IEnumerable<ProgramDto> AsDto(this Program program)
        {
            var programs = new List<ProgramDto>();
            var programDto = new ProgramDto
            {
                Category = program.Category,
                Description = program.Description,
                Id = program.Id,
                Leaders = program.Leaders,
                Photo = program.Photo,
                Sounds = program.Sounds,
                AntenaId = program.AntenaId,
                ArticleLink = program.ArticleLink,
                IsActive = program.IsActive,
                StartHour = program.StartHour,
                StopHour = program.StopHour,
                Title = program.Description
            };
            programs.Add(programDto);
            var subPrograms = program.Subprograms.AsDto(program);
            programs.AddRange(subPrograms);
            return programs;
        }

        private static IEnumerable<ProgramDto> AsDto(this IEnumerable<SubProgram> subPrograms, Program program)
        {
            return subPrograms.Select(s => new ProgramDto
            {
                Category = program.Category,
                Description = s.Description,
                Id = s.Id,
                Leaders = s.Leaders,
                Photo = s.Photo,
                Sounds = s.Sounds,
                AntenaId = program.AntenaId,
                ArticleLink = program.ArticleLink,
                IsActive = s.IsActive,
                StartHour = s.StartHour,
                StopHour = s.StopHour,
                Title = s.Title
            });
        }
    }
}