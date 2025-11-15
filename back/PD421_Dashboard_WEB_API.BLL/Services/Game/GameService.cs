using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PD421_Dashboard_WEB_API.BLL.Dtos.Game;
using PD421_Dashboard_WEB_API.BLL.Services.BlobStorage;
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

        public async Task<ServiceResponse> CreateGameAsync(CreateGameDto dto, string storagePath)
        {

            // отримуємо жанри
            var genres = await _gameRepository.GetGenresByIdsAsync(dto.GenreIds);

            // мапимо dto в entity і даємо їй жанри
            var entity = _mapper.Map<GameEntity>(dto);
            entity.Genres = genres;

            // створюємо папку для збереження зображень гри
            string folderName = entity.Id.ToLower();
            // зберігаємо головне зображення
            var mainImageName = await _storageService.SaveImageAsync(dto.MainImage,storagePath);

            // перевіряємо чи збереження пройшло успішно
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

            // Зберігаємо зображення
            var imageNames = await _storageService.SaveImagesAsync(dto.Images, folderName);


            if (imageNames.Count() > 0)
            {
                // ???
                var images = imageNames.Select(name => new GameImageEntity
                {
                    GameId = entity.Id,
                    IsMain = false,
                    ImagePath = Path.Combine(entity.Id, name.ToLower())
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
            var game = await _gameRepository.GetByIdAsync(id.ToLower());
            if (game != null)
            {
                // передаємо айди як контейнер того що назва контейнера = айди гри
                await _storageService.DeleteImageContainer(id.ToLower());
                await _gameRepository.DeleteAsync(game);
                return new ServiceResponse { Message = "Гру успішно видалено", IsSuccess = true };
            }
            return new ServiceResponse { Message = "Гру не знайдено", IsSuccess = false, HttpStatusCode = HttpStatusCode.BadRequest };
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
            var game = await _gameRepository.GetByIdAsync(id.ToLower());
            if (game != null)
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
            var entity = await _gameRepository.GetByIdAsync(dto.Id.ToLower());
            if (entity == null)
            {
                return new ServiceResponse
                {
                    Message = $"Гру з id '{dto.Id.ToLower()}' не знайдено",
                    IsSuccess = false,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
            }

            _mapper.Map(dto, entity);

            if (dto.MainImage != null)
            {
                var currentMain = entity.Images.FirstOrDefault(i => i.IsMain);
                if (currentMain != null)
                {
                    await _storageService.DeleteImageAsync(dto.Id.ToLower(), imagesPath);
                    entity.Images.Remove(currentMain);
                }
                var mainImagePath = await _storageService.SaveImageAsync(dto.MainImage, dto.Id.ToLower());
                entity.Images.Add(new GameImageEntity { GameId = entity.Id, ImagePath = mainImagePath!, IsMain = true });
            }

            await _storageService.SaveImagesAsync(dto.Images, imagesPath.ToLower());

            await _gameRepository.UpdateAsync(entity);
            
            return new ServiceResponse
            {
                Message = "Гру успішно оновлено",
                IsSuccess = true,
                HttpStatusCode = HttpStatusCode.OK
            };


            // путь до збереження зображень
            //string gamePath = Path.Combine(imagesPath, entity.Id);
            //Directory.CreateDirectory(gamePath);

            //// Save main image
            //if(dto.MainImage != null)
            //{
            //    var mainImageName = await _storageService.SaveImageAsync(dto.MainImage!, gamePath);

            //    var mainImage = new GameImageEntity
            //    {
            //        GameId = entity.Id,
            //        ImagePath = Path.Combine(entity.Id, mainImageName),
            //        IsMain = true
            //    };
            //     entity.Images.Add(mainImage);
            //}




            // Save images
            //var imageNames = await _storageService.SaveImagesAsync(dto.Images, gamePath);

            //if (imageNames.Count() > 0)
            //{
            //    var images = imageNames.Select(name => new GameImageEntity
            //    {
            //        GameId = entityMap.Id,
            //        IsMain = false,
            //        ImagePath = Path.Combine(entity.Id, name)
            //    });

            //    foreach (var i in images)
            //    {
            //        entity.Images.Add(i);
            //    }
        }

    };

}
    

