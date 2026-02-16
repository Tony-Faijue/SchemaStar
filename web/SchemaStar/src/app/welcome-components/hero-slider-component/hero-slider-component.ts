import { CommonModule } from '@angular/common';
import { Component, signal, OnInit, OnDestroy } from '@angular/core';

export interface HeroSlide{
  title: string,
  description: string,
  buttonText: string,
  buttonUrl: string,
  image: string
}

@Component({
  selector: 'app-hero-slider-component',
  imports: [CommonModule],
  templateUrl: './hero-slider-component.html',
  styleUrl: './hero-slider-component.css',
})
export class HeroSliderComponent implements OnInit, OnDestroy {
  
  public currentSlide = signal<number>(0);
  public autoPlay = signal<boolean>(true);
  private intervalId: any;

  public slides:HeroSlide[] = [
    {
      title: "Slide A",
      description: "Description A",
      buttonText: "Click Me",
      buttonUrl: "#",
      image: "https://images.unsplash.com/photo-1515879218367-8466d910aaa4?w=1000&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8Y29kZXxlbnwwfHwwfHx8MA%3D%3D"
    },
     {
      title: "Slide B",
      description: "Description B",
      buttonText: "Click Me",
      buttonUrl: "#",
      image: "https://images.unsplash.com/photo-1551958219-acbc608c6377?q=80&w=1170&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"
    },
     {
      title: "Slide C",
      description: "Description C",
      buttonText: "Click Me",
      buttonUrl: "#",
      image: "https://images.unsplash.com/photo-1588195538326-c5b1e9f80a1b?w=1000&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8Y2FrZXxlbnwwfHwwfHx8MA%3D%3D"
    }
  ];

  ngOnInit(): void{
    this.startAutoPlay();
  }

  ngOnDestroy(): void {
      this.stopAutoPlay();
  }

  /**
   * Set the current slide to the next
   */
  next(): void {
    this.currentSlide.update((value) => (value + 1) % this.slides.length);
  }

  /**
   * Set the current slide to the previous
   */
  prev(): void {
    this.currentSlide.update((value) => (value - 1 + this.slides.length) % this.slides.length)
  }

  /**
   * Set the current slide to the one located at index
   * @param index 
   */
  goTo(index: number){
    this.currentSlide.set(index);
  }

  /**
   * Automatically loop through the slides at a given time interval
   */
  startAutoPlay(){
    this.intervalId = setInterval(() => {
      if(this.autoPlay()){
        this.next();
      }
    }, 5000);
  }

  /**
   * Stop the automatic looping through the slides
   */
  stopAutoPlay(){
    if (this.intervalId){
      clearInterval(this.intervalId);
    }
  }

  handleImageError(event: any){
    event.target.src = 'https://images.unsplash.com/photo-1633078654544-61b3455b9161?q=80&w=1045&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D';
  }
  
}
