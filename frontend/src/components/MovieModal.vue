<template>
    <v-dialog :model-value="show" @update:model-value="close" max-width="500px">
      <v-card>
        <v-card-title>{{ isEditMode ? "Edytuj film" : "Dodaj film" }}</v-card-title>
        <v-card-text>
          <v-form class="space-y-2">
            <v-text-field
              v-model="localMovie.title"
              label="Tytuł"
              :error-messages="v$.title.$dirty ? v$.title.$errors.map(e => e.$message) : []"
              @blur="v$.title.$touch()"
              required
            ></v-text-field>
            <v-text-field
              v-model="localMovie.year"
              label="Rok"
              :error-messages="v$.year.$dirty ? v$.year.$errors.map(e => e.$message) : []"
              @blur="v$.year.$touch()"
              required
            ></v-text-field>
            <v-text-field
              v-if="!isEditMode"
              v-model="localMovie.director"
              label="Reżyser"
              :error-messages="v$.director.$dirty ? v$.director.$errors.map(e => e.$message) : []"
              @blur="v$.director.$touch()"
              required
            ></v-text-field>
            <v-text-field
              v-if="!isEditMode"
              v-model="localMovie.rate"
              label="Ocena"
              :error-messages="v$.rate.$dirty ? v$.rate.$errors.map(e => e.$message) : []"
              @blur="v$.rate.$touch()"
              required
            ></v-text-field>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-btn color="blue darken-1" text @click="close">Anuluj</v-btn>
          <v-btn 
            color="blue darken-1" 
            text 
            @click="save" 
            :disabled="v$.$error"
          >Zapisz</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </template>
  
  <script setup>
  import { ref, onMounted, defineProps, defineEmits, watch } from 'vue';
  import { useVuelidate } from '@vuelidate/core';
  import { required, maxLength, between } from '@vuelidate/validators';
  
  const props = defineProps({
    show: Boolean,
    movie: Object,
    isEditMode: Boolean,
  });
  
  const emit = defineEmits(['update:model-value', 'save']);
  
  const rules = {
    title: { 
      required, 
      maxLength: maxLength(200) 
    },
    year: { 
      required,
      between: between(1900, 2200) 
    },
    director: { 
      required 
    },
    rate: { 
      required,
      between: between(1, 10) 
    }
  };
  
  const localMovie = ref({ ...props.movie });
  const v$ = useVuelidate(rules, localMovie);
  
  watch(() => props.show, (newValue) => {
    if (newValue) {
      localMovie.value = { ...props.movie };
    }
    v$.value.$reset();
  }, { immediate: true });
  
  watch(() => props.movie, (newMovie) => {
    localMovie.value = { ...newMovie };
  }, { deep: true });
  
  const close = () => {
    emit('update:model-value', false);
    v$.value.$reset();
  };
  
  const save = async () => {
    const isValid = await v$.value.$validate();
    if (isValid) {
      emit('save', localMovie.value);
      close();
    }
  };

  onMounted(() => {
    console.log('localMovie', localMovie);
  });
  </script>