
import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { imagesUrl } from "../../env";
import type { ServiceResponse } from "../../services/types";
import type { Image } from "../slices/types";

export const imageApi = createApi({
    reducerPath: 'image',
    baseQuery: fetchBaseQuery({
        baseUrl: `${imagesUrl}/image`
    }),
    tagTypes: ['Images'],
    endpoints: (build) => ({
        getByName: build.query<ServiceResponse<Image>,string>({
            query: (fileName) => ({url:`?fileName=${fileName}`, method:"get"}),
        })
    })
});


export const {useGetByNameQuery} = imageApi


