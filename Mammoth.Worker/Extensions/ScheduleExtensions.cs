using System.Collections.Generic;
using System.Linq;
using Mammoth.Core.Entities;
using Mammoth.Worker.DTO;

namespace Mammoth.Worker.Extensions
{
    public static class ScheduleExtensions
    {
        public static IEnumerable<ProgramDto> AsDto(this Program program)
        {
            var programs = new List<ProgramDto>();
            var subprograms = program.Subprograms.ToList();
            var subprogramsDto = subprograms.AsDto(program).ToList();
            if (subprogramsDto.Any())
            {
                var first = subprogramsDto.First();
                if (first.StartHour < program.StartHour)
                {
                    var opening = new ProgramDto
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
                        StopHour = first.StartHour,
                        Title = program.Description
                    };
                    programs.Add(opening);
                }

                programs.AddRange(subprogramsDto);
                var last = subprogramsDto.Last();
                if (last.StopHour >= program.StopHour) return programs;
                var closing = new ProgramDto
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
                    StartHour = last.StopHour,
                    StopHour = program.StopHour,
                    Title = program.Description
                };
                programs.Add(closing);
            }
            else
            {
                programs.Add(new ProgramDto
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
                });
            }

            return programs;
        }

        public static ProgramDto AsDto(this SubProgram subProgram, Program program)
        {
            return new ProgramDto
            {
                Category = program.Category,
                Description = subProgram.Description,
                Id = subProgram.Id,
                Leaders = subProgram.Leaders,
                Photo = subProgram.Photo,
                Sounds = subProgram.Sounds,
                AntenaId = program.AntenaId,
                ArticleLink = program.ArticleLink,
                IsActive = subProgram.IsActive,
                StartHour = subProgram.StartHour,
                StopHour = subProgram.StopHour,
                Title = subProgram.Description
            };
        }

        public static IEnumerable<ProgramDto> AsDto(this IEnumerable<Program> programs)
        {
            return programs.Select(AsDto).SelectMany(p => p);
        }

        public static IEnumerable<ProgramDto> AsDto(this IEnumerable<SubProgram> subPrograms, Program program)
        {
            return subPrograms.Select(x => x.AsDto(program));
        }

        public static IEnumerable<Track> AsEntity(this IEnumerable<ProgramDto> programs)
        {
            return programs.Select(x => x.AsEntity());
        }

        public static Track AsEntity(this ProgramDto program)
        {
            return new Track(program.AntenaId, program.ArticleLink, program.Category, program.Description, program.Id,
                program.Title, program.IsActive, program.Leaders, program.Photo, program.Sounds, program.StartHour,
                program.StopHour);
        }
    }
}