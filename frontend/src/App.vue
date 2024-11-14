<template>
    <MovieList
        :movies="movies"
        @open-modal="openModal"
        @fetch-movies="fetchMovies"
        @show-message="showMessage" />
    <MovieModal
        v-model="showModal"
        :movie="selectedMovie"
        :isEditMode="isEditMode"
        @close="closeModal"
        @save="handleSave" />
    <v-snackbar
        v-model="snackbar.show"
        :color="snackbar.color"
        location="top right"
        :timeout="3000">
        {{ snackbar.text }}
    </v-snackbar>
</template>

<script setup>
import { ref } from "vue";
import MovieList from "./components/MovieList.vue";
import MovieModal from "./components/MovieModal.vue";
import axios from "./axios";

const showModal = ref(false);
const selectedMovie = ref({});
const isEditMode = ref(false);
const movies = ref([]);
const snackbar = ref({
    show: false,
    text: "",
    color: "success",
});

const fetchMovies = async () => {
    try {
        const response = await axios.get("/api/movies");
        movies.value = response.data;
    } catch (error) {
        showMessage("Error fetching movies", "error");
        console.error("Błąd podczas pobierania filmów:", error);
    }
};

const openModal = (movie = {}) => {
    console.log("movieedit", movie);
    selectedMovie.value = { ...movie };
    isEditMode.value = !!movie.id;
    showModal.value = true;
};

const closeModal = () => {
    showModal.value = false;
};

const showMessage = (text, color = "success") => {
    snackbar.value = {
        show: true,
        text,
        color,
    };
};

const handleSave = async (movie) => {
    try {
        if (isEditMode.value) {
            await axios.put(`/api/movies/${movie.id}`, movie);
            showMessage("Film zaktualizowany pomyślnie");
        } else {
            await axios.post("/api/movies", movie);
            showMessage("Film dodany pomyślnie");
        }
        showModal.value = false;
        fetchMovies();
    } catch (error) {
        showMessage("Błąd podczas zapisywania filmu", "error");
        console.error(error);
    }
};
</script>
