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

        public async Task<ServiceResponse> CreateGameAsync(CreateGameDto dto)
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


            // Save main image
            var mainImageName = await _storageService.SaveImageAsync(dto.MainImage, entity.Id );

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
            var imageNames = await _storageService.SaveImagesAsync(dto.Images, entity.Id);

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

        public async Task<ServiceResponse> DeleteGameAsync(string id)
        {
            var entity = await _gameRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new ServiceResponse { Message = "Гру не знайдено", IsSuccess = false , HttpStatusCode = HttpStatusCode.BadRequest};
            }
            var mainImage = entity.Images.FirstOrDefault(i => i.IsMain == true);
            if(mainImage != null)
            {
                await _storageService.DeleteImageAsync(mainImage.ImagePath);
            }
            var images = entity.Images.Where(i => !i.IsMain);
            if(images != null)
            {
                foreach(var image in images)
                {
                    await _storageService.DeleteImageAsync(image.ImagePath);
                }
            }
            await _gameRepository.DeleteAsync(entity);
            return new ServiceResponse { Message = "Гру успішно видалено", IsSuccess=true, HttpStatusCode= HttpStatusCode.OK };

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

        public async Task<ServiceResponse> GetImageUrlByFileName(string fileName)
        {
            var imageUrl = await _storageService.GetImageUrlByFileName(fileName);
            if(String.IsNullOrWhiteSpace(imageUrl))
            {
                return new ServiceResponse { Message = "Картинку не знайдено!",IsSuccess=false, HttpStatusCode = HttpStatusCode.NotFound };
            }
            return new ServiceResponse { Message = "Картинку знайдено!", IsSuccess = true, HttpStatusCode = HttpStatusCode.OK, Data=imageUrl };
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
