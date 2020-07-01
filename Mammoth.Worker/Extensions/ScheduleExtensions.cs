using System;
using System.Collections.Generic;
using System.Linq;
using Mammoth.Core.Entities;
using Mammoth.Worker.DTO;

namespace Mammoth.Worker.Extensions
{
    public static class ScheduleExtensions
    {
        public static IEnumerable<ProgramDto> AsDto(this IEnumerable<Program> programs)
        {
            return programs.Select(AsDto).SelectMany(p => p);
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

            var subPrograms = program.Subprograms.AsDto(program).ToList();
            // if (subPrograms.Any())
            // {
            //     var first = subPrograms.First();
            //     program.StopHour = first.StartHour;
            // }
            if (!subPrograms.Any())
            {
                programs.Add(programDto);
            }
            else
            {
                programs.AddRange(subPrograms);
            }

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

        public static IEnumerable<Track> AsEntity(this IEnumerable<ProgramDto> programs)
            => programs.Select(x => x.AsEntity());

        public static Track AsEntity(this ProgramDto program)
            => new Track(program.AntenaId, program.ArticleLink, program.Category, program.Description, program.Id,
                program.Title, program.IsActive, program.Leaders, program.Photo, program.Sounds, program.StartHour,
                program.StopHour);
    }
}