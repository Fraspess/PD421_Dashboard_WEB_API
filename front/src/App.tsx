import "./App.css";
import { Route, Routes } from "react-router-dom";
import DefaultLayout from "./components/layout/DefaultLayout";
import MainPage from "./pages/mainPage/MainPage";
import LoginPage from "./pages/loginPage/LoginPage";
import { useEffect } from "react";
import { loginSucess } from "./store/slices/authSlice";
import { useDispatch } from "react-redux";
import GameListPage from "./pages/gamePage/GameListPage";
import CreateGamePage from "./pages/gamePage/CreateGamePage";
import RegisterPage from "./pages/registerPage/RegisterPage";
import DetailsPage from "./pages/gamePage/DetailsPage";

function App() {
    const dispatch = useDispatch();

    useEffect(() => {
        // read jwt token
        const token = localStorage.getItem("token");
        if(token) {
            dispatch(loginSucess(token));
        }
    }, []);

    return (
        <>
            <Routes>
                <Route path="/" element={<DefaultLayout />}>
                    <Route index element={<MainPage />} />
                    <Route path="/login" element={<LoginPage />} />
                      <Route path="/game">
                        <Route index element={<GameListPage />} />
                        <Route path="create" element={<CreateGamePage />} />
                        <Route path=":id" element={<DetailsPage />} />
                    </Route>
                    <Route path="/register" element={<RegisterPage />} />
                    
                </Route>
            </Routes>
        </>
    );
}

export default App;
