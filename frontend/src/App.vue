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
        showMessage("Błąd podczas pobierania filmów.", "error");
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
    const endpoint = isEditMode.value ? `/api/movies/${movie.id}` : '/api/movies';
    const method = isEditMode.value ? 'put' : 'post';
    const successMessage = isEditMode.value ? 'Film zaktualizowany pomyślnie' : 'Film dodany pomyślnie';
    const errorMessage = isEditMode.value ? 'Błąd podczas aktualizowania filmu' : 'Błąd podczas dodawania filmu';

    try {
        await axios[method](endpoint, movie);
        showMessage(successMessage);
        showModal.value = false;
        fetchMovies();
    } catch (error) {
        console.error(`${errorMessage}:`, error);
        showMessage(errorMessage, 'error');
    }
};
</script>
