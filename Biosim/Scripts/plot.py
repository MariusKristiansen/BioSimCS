import matplotlib.pyplot as plt
import numpy as np
import sys

try:
	data = np.genfromtxt("../../../Results/SimResult.csv", delimiter=",", names=["Year", "Herbivores", "Carnivores", "HFitness", "CFitness", "HerbivoreAvgAge", "CarnivoreAvgAge", "HerbivoreAvgWeight", "CarnivoreAvgWeight","HerbivoreBirths","CarnivoreBirths"])
	fig = plt.figure(1)
	ax1 = fig.add_subplot(511)
	ax2 = fig.add_subplot(512)
	ax3 = fig.add_subplot(513)
	ax4 = fig.add_subplot(514)
	ax5 = fig.add_subplot(515)

	plt.xlabel('Year')

	ax1.set_title('Population')
	ax1.plot(data["Year"], data["Herbivores"], label='Herbivores')
	ax1.plot(data["Year"], data["Carnivores"], label='Carnivores')
	ax1.legend()

	ax2.set_title('Fitness')
	ax2.plot(data["Year"], data["HFitness"], label='Herbivore')
	ax2.plot(data["Year"], data["CFitness"], label='Carnivore')
	ax2.legend()

	ax3.set_title('Average age')
	ax3.plot(data["Year"], data["HerbivoreAvgAge"], label='Herbivore')
	ax3.plot(data["Year"], data["CarnivoreAvgAge"], label='Carnivore')
	ax3.legend()

	ax4.set_title('Average Weight')
	ax4.plot(data["Year"], data["HerbivoreAvgWeight"], label='Herbivore')
	ax4.plot(data["Year"], data["CarnivoreAvgWeight"], label='Carnivore')
	ax4.legend()

	ax5.set_title('Births')
	ax5.plot(data["Year"], data["HerbivoreBirths"], label='Herbivore')
	ax5.plot(data["Year"], data["CarnivoreBirths"], label='Carnivore')
	ax5.legend()

	plt.subplots_adjust(left=0.1, 
                    bottom=0.1,  
                    right=0.9,  
                    top=0.9,  
                    wspace=0,  
                    hspace=.8) 
	plt.savefig("../../../Results/graph.png")
	plt.show()
	#DPI = plt.get_dpi()
	#fig.set_size_inches(1000.0/float(DPI),1000.0/float(DPI))
	#plt.savefig("../../../Results/graph.png")
except Exception as e:
	print(e) #To be able to see error messages from external view
	input()

