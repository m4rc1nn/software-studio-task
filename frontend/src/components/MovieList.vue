<template>
    <div class="mt-12 max-w-7xl mx-auto">
        <v-container>
            <v-row>
                <v-col cols="12">
                    <v-btn
                        class="mr-4"
                        @click="openAddModal"
                        >Dodaj film</v-btn
                    >
                    <v-btn @click="fetchMoviesFromExternalAPI"
                        >Pobierz filmy</v-btn
                    >
                </v-col>
            </v-row>
            <v-data-table
                :items="movies"
                :headers="headers"
                item-key="id"
                :hide-default-footer="true"
                class="border mt-6">
                <template v-slot:headers>
                    <tr>
                        <th
                            v-for="header in headers"
                            :key="header.value"
                            class="text-center">
                            {{ header.text }}
                        </th>
                    </tr>
                </template>
                <template v-slot:item="{ item }">
                    <tr class="py-4">
                        <td
                            v-for="header in headers"
                            :key="header.value"
                            class="text-center py-4">
                            <template v-if="header.value === 'actions'">
                                <v-btn
                                    icon
                                    @click="openEditModal(item)"
                                    class="mr-4">
                                    <v-icon>mdi-pencil-outline</v-icon>
                                </v-btn>
                                <v-btn
                                    icon
                                    color="red"
                                    @click="confirmDelete(item.id)">
                                    <v-icon>mdi-delete</v-icon>
                                </v-btn>
                            </template>
                            <template v-else>
                                {{ item[header.value] }}
                            </template>
                        </td>
                    </tr>
                </template>
            </v-data-table>
        </v-container>
    </div>
</template>

<script>
import { ref, onMounted } from "vue";
import axios from "../axios";

export default {
    props: {
        movies: {
            type: Array,
            required: true,
            default: () => [],
        },
    },
    setup(props, { emit }) {
        const headers = ref([
            { text: "ID", value: "id", align: "center" },
            { text: "Tytuł", value: "title", align: "center" },
            { text: "Rok", value: "year", align: "center" },
            { text: "Reżyser", value: "director", align: "center" },
            { text: "Ocena", value: "rate", align: "center" },
            { text: "Akcje", value: "actions", align: "center" },
        ]);

        const openAddModal = () => {
            emit("open-modal", {});
        };

        const openEditModal = (movie) => {
            console.log("movie", movie);
            emit("open-modal", movie);
        };

        const confirmDelete = async (movieId) => {
            if (confirm("Czy na pewno chcesz usunąć ten film?")) {
                await axios.delete(`/api/movies/${movieId}`);
                emit("show-message", "Film usunięty pomyślnie", "success");
                emit("fetch-movies");
            }
        };

        const fetchMoviesFromExternalAPI = async () => {
            emit("show-message", `Pobieranie filmów z zewnętrznej bazy...`, "info");
            const response = await axios.post("/api/movies/fetch-external");
            if (response.status === 200) {
                emit("show-message", `Poprawnie pobrano filmy z zewnętrznej bazy. Nowych filmów: ${response.data.newMoviesCount}`, "success");
                emit("fetch-movies");
            }
        };

        onMounted(() => {
            emit("fetch-movies");
        });

        return {
            headers,
            openAddModal,
            openEditModal,
            confirmDelete,
            fetchMoviesFromExternalAPI,
        };
    },
};
</script>
