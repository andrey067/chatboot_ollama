using System.Net.Http.Json;

namespace Chatbot.App.Handlers;

public class ApiHandler(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);

    public async Task<BaseResponse<Category?>> CreateCategoryHandlerAsync(CreateCategoryRequest request)
    {
        var result = await _client.PostAsJsonAsync("v1/categories", request);
        return await result.Content.ReadFromJsonAsync<BaseResponse<Category?>>()
            ?? new BaseResponse<Category?>(null, 400, "Falha ao criar categoria");
    }

    public async Task<BaseResponse<Category?>> UpdateCategoryHandlerAsync(UpdateCategoryRequest request)
    {
        var result = await _client.PutAsJsonAsync($"v1/categories/{request.Id}", request);
        return await result.Content.ReadFromJsonAsync<BaseResponse<Category?>>()
               ?? new BaseResponse<Category?>(null, 400, "Falha ao atualizar a categoria");
    }

    public async Task<BaseResponse<Category?>> DeleteCategoryHandlerAsync(DeleteCategoryRequest request)
    {
        var result = await _client.DeleteAsync($"v1/categories/{request.Id}");
        return await result.Content.ReadFromJsonAsync<BaseResponse<Category?>>()
               ?? new BaseResponse<Category?>(null, 400, "Falha ao excluir a categoria");
    }

    public async Task<BaseResponse<Category?>> GetByIdCategoryHandlerAsync(GetCategoryByIdRequest request)
        => await _client.GetFromJsonAsync<BaseResponse<Category?>>($"v1/categories/{request.Id}")
           ?? new BaseResponse<Category?>(null, 400, "Não foi possível obter a categoria");

    public async Task<PagedResponse<List<Category>?>> GetAllCategoryHandlerAsync(GetAllCategoriesRequest request)
        => await _client.GetFromJsonAsync<PagedResponse<List<Category>?>>("v1/categories")
           ?? new PagedResponse<List<Category>?>(null, 400, "Não foi possível obter as categorias");
}