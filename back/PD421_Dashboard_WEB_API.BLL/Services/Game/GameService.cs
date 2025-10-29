using AutoMapper;
using PD421_Dashboard_WEB_API.BLL.Dtos.Game;
using PD421_Dashboard_WEB_API.BLL.Services.Storage;
using PD421_Dashboard_WEB_API.DAL.Entitites;
using PD421_Dashboard_WEB_API.DAL.Repositories.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PD421_Dashboard_WEB_API.BLL.Services.Game
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageService;
        public GameService(IGameRepository gameRepository, IMapper mapper, IStorageService storageService)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
            _storageService = storageService;
        }

        public async Task<ServiceResponse> CreateGameAsync(CreateGameDto dto,string imagesPath)
        {
            var genres = await _gameRepository.GetGenresByIdsAsync(dto.GenreIds);
            //var entity = new GameEntity()
            //{
            //    Name = dto.Name,
            //    Description = dto.Description,
            //    Price = dto.Price,
            //    ReleaseDate = dto.ReleaseDate,
            //    Publisher = dto.Publisher,
            //    Developer = dto.Developer,
            //    Genres = genres,
            //};
            var entity = _mapper.Map<GameEntity>(dto);
            entity.Genres = genres;
            string gamePath = Path.Combine(imagesPath, entity.Id);
            Directory.CreateDirectory(gamePath);

            // Save main image
            var mainImageName = await _storageService.SaveImageAsync(dto.MainImage, gamePath);

            if (mainImageName == null)
            {
                return new ServiceResponse
                {
                    IsSuccess = false,
                    HttpStatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Message = "Помилка під час збереження головної картинки"
                };

            }

            var mainImage = new GameImageEntity
            {
                GameId = entity.Id,
                ImagePath = Path.Combine(entity.Id, mainImageName),
                IsMain = true
            };

            entity.Images.Add(mainImage);

            // Save images
            var imageNames = await _storageService.SaveImagesAsync(dto.Images, gamePath);

            if (imageNames.Count() > 0)
            {
                var images = imageNames.Select(name => new GameImageEntity
                {
                    GameId = entity.Id,
                    IsMain = false,
                    ImagePath = Path.Combine(entity.Id, name)
                });

                foreach (var i in images)
                {
                    entity.Images.Add(i);
                }
            }

            await _gameRepository.CreateAsync(entity);
            return new ServiceResponse
            {
                Message = "Гру успішно створено",
                IsSuccess = true,
                HttpStatusCode = HttpStatusCode.Created
            };
        }

        public async Task<ServiceResponse> DeleteGameAsync(string id,string imagesPath)
        {
            var game = await _gameRepository.GetByIdAsync(id);
            if(game != null)
            {
                string fullImagesFolderPath = Path.Combine(imagesPath,game.Id);
                await _storageService.DeleteImageAsync(fullImagesFolderPath);
                await _gameRepository.DeleteAsync(game);
                return new ServiceResponse { Message = "Гру успішно видалено", IsSuccess = true };
            }
            return new ServiceResponse { Message = "Гру не знайдено", IsSuccess = false , HttpStatusCode = HttpStatusCode.BadRequest};
        }

        public async Task<ServiceResponse> GetAllGamesAsync()
        {
            var games = await _gameRepository.GetAllGamesAsync();
            var dtos = _mapper.Map<List<GameDto>>(games);
            return new ServiceResponse
            {
                Message = "Ігри успішно отримано",
                IsSuccess = true,
                Data = dtos,
                HttpStatusCode = HttpStatusCode.OK
            };
        }


        public async Task<ServiceResponse> GetGameByIdAsync(string id)
        {
            var game = await _gameRepository.GetByIdAsync(id);
            if(game != null)
            {
                return new ServiceResponse
                {
                    Message = "Гру успішно знайдено",
                    IsSuccess = true,
                    Data = _mapper.Map<GameDto>(game),
                    HttpStatusCode = HttpStatusCode.OK
                };
            }
            return new ServiceResponse
            {
                Message = "Гру не знайдено",
                IsSuccess = false,
                HttpStatusCode = HttpStatusCode.NotFound
            };
        }

        public async Task<ServiceResponse> GetGamesByGenreAsync(string genreName)
        {
            var games = await _gameRepository.GetGamesByGenreAsync(genreName);
            games = _mapper.Map<List<GameEntity>>(games);
            return new ServiceResponse
            {
                Message = "Ігри успішно отримано",
                IsSuccess = true,
                Data = _mapper.Map<List<GameDto>>(games),
                HttpStatusCode = HttpStatusCode.OK
            };
        }

        public async Task<ServiceResponse> UpdateGameAsync(UpdateGameDto dto, string imagesPath)
        {
            var entity = await _gameRepository.GetByIdAsync(dto.Id);
            if (entity == null)
            {
                return new ServiceResponse
                {
                    Message = $"Гру з id '{dto.Id}' не знайдено",
                    IsSuccess = false,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }
            entity = _mapper.Map(dto, entity);

            string gamePath = Path.Combine(imagesPath, entity.Id);
            Directory.CreateDirectory(gamePath);

            // Save main image
            if(dto.MainImage != null)
            {
            var mainImageName = await _storageService.SaveImageAsync(dto.MainImage!, gamePath);

            var mainImage = new GameImageEntity
            {
                GameId = entity.Id,
                ImagePath = Path.Combine(entity.Id, mainImageName),
                IsMain = true
            };
            entity.Images.Add(mainImage);
            }


            // Save images
            var imageNames = await _storageService.SaveImagesAsync(dto.Images, gamePath);

            if (imageNames.Count() > 0)
            {
                var images = imageNames.Select(name => new GameImageEntity
                {
                    GameId = entity.Id,
                    IsMain = false,
                    ImagePath = Path.Combine(entity.Id, name)
                });

                foreach (var i in images)
                {
                    entity.Images.Add(i);
                }
            }

            await _gameRepository.UpdateAsync(entity);
            return new ServiceResponse
            {
                Message = "Гру успішно оновлено",
                IsSuccess = true,
                HttpStatusCode = HttpStatusCode.OK
            };

        }
    }
}
