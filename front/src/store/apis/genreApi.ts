
import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { apiBaseGenreUrl} from "../../env";
import type { ServiceResponse } from "../../services/types";
import type { Genre } from "../slices/types";

export const genreApi = createApi({
    reducerPath: 'genre',
    baseQuery: fetchBaseQuery({
        baseUrl: `${apiBaseGenreUrl}`
    }),
    tagTypes: ['Genres'],
    endpoints: (build) => ({
        getGenres: build.query<ServiceResponse<Genre[]>, null>({
            query: () => ({ url: '', method: "get" }),
            providesTags: ['Genres']
        }),
    })
});


export const  getGenres  = genreApi;