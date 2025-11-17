import { useGetByIdQuery } from "../../store/apis/gameApi";
import { useParams } from "react-router-dom";
import { LinearProgress, Box, Typography, Divider } from "@mui/material";
import { imagesUrl } from "../../env";

const DetailsPage = () => {
    const { id } = useParams();
    const { data, isLoading } = useGetByIdQuery(`${id}`);

    if (isLoading) return <LinearProgress />;
    if (!data) return <div>No data</div>;
    const game = data.data;
    if(!game) return <div>Game not found</div>;

    console.log(data);

    return <>
        <Box sx={{display: 'flex', flexDirection: 'row', gap: 2, p: 2, flex:1}}>
            <Box sx={{display:"flex", flex:"0 0 60%", flexDirection:"column", gap:2,height:"100%"}}>
              <Box sx={{height:"50px"}}>
                <Typography variant="h4">{game.name}</Typography>
              </Box>
                <img src={`${game.mainImage && game.mainImage.imagePath
                                            ? imagesUrl + game.mainImage.imagePath
                                            : `${imagesUrl}default.png`
                                    }`} 
                                    alt={game.name} style={{width: '70%', height: '40%', borderRadius: '8px'}} />
                <Box sx={{display:"flex", flexDirection:"row"}}>
                    {game.images && game.images.map((img, index) => (
                        <img key={index} src={`${imagesUrl + img.imagePath}`} alt={`${game.name} ${index}`} style={{width: '20%', height: '60%', borderRadius: '8px'}} />
                    ))}
                </Box>
                
            </Box>
            <Box sx={{flex:1, flexDirection:"column"}}>
              <Box sx={{height:"50px"}}></Box>
              <Box sx={{display:"flex",flexDirection:"column",flex:2}}>
                    <Typography variant="h5">Description</Typography>
                    <Divider></Divider>
                    <Typography variant="h6">{game.description}</Typography>
                    <Divider></Divider>
                    <Typography variant="h6">Genres: {getGenreNames(game)}</Typography>
                    <Divider></Divider>
                    <Typography variant="h6">Developer:{game.developer}</Typography>
                    <Divider></Divider>
                    <Typography variant="h6">Publisher:{game.publisher}</Typography>
                    <Divider></Divider>
                    <Typography variant="h6">Price: ${game.price}</Typography>
                    <Divider></Divider>
                    <Typography variant="h6">Release Date: {new Date(game.releaseDate).toLocaleDateString()}</Typography>
                    <Divider></Divider>
              </Box>
            </Box>  
        </Box>
    </>
}

const getGenreNames = (game: any) => {
    return game.genres.map((g: any) => g.name).join(", ");
}



export default DetailsPage;