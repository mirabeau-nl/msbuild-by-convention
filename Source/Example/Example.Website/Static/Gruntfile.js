module.exports = function(grunt) {
  
  grunt.initConfig({
    
    pkg: grunt.file.readJSON('package.json'),

    watch: {
      scss: {
        files: ['scss/**/*.scss'],
        tasks: ['build'],
        options: {
          spawn: false
        }
      },
      dev: {
        files: ['css/application.css'],
        options: {
          livereload: true,
          spawn: false
        }
      }
    },

    sass: {
      dev: {
        options: {
          style: 'expanded',
          trace: true
        },
        files: [{
          expand: true,
          cwd: 'scss/',
          src: ['application.scss'],
          dest: 'css/',
          ext: '.css'
        }]
      },
      prod: {
        options: {
          style: 'compressed',
          noCache: true
        },
        files: [{
          expand: true,
          cwd: 'scss/',
          src: ['application.scss'],
          dest: 'css/',
          ext: '.css'
        }]
      }
    },

    autoprefixer: {
      options: {
        browsers: ["> 1%", "last 2 versions", "ie >= 8"]
      },
      dist: {
        expand: true,
        flatten: true,
        src: 'css/application.css',
        dest: 'css/'
      }
    },

    csso: {
      options: {
        restructure: false,
        report: 'min'
      },
      dist: {
        files: {
          'css/application-min.css': ['css/application.css']
        }
      }
    },
  });

  // load plugins
  [
    'grunt-contrib-sass',
    'grunt-contrib-watch',
    'grunt-autoprefixer',
    'grunt-csso'
  ].forEach(function(package) {
    grunt.loadNpmTasks(package);
  });

  // register tasks
  grunt.registerTask('build', [
    'sass:dev',
    'autoprefixer',
    // 'csso'
  ]);

  grunt.registerTask('default', [
    'build',
    'watch'
  ]);
};
